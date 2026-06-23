using System.Collections;
using System.Collections.Generic;
using CSILib.SoundManager.RunTime;
using UnityEngine;
using UnityEngine.Audio;

namespace csiimnida.CSILib.SoundManager.RunTime
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
    
        [SerializeField] private SoundListSo _soundListSo;
        [SerializeField] private AudioMixer _mixer;

        [Tooltip("같은 이름 단발(one-shot) 사운드의 동시 재생 최대 개수. 연사 등으로 무한정 쌓여 오디오 보이스 한도를 넘기는 것을 막는다.")]
        [SerializeField] private int maxConcurrentPerSound = 8;

        private Dictionary<string, AudioSource> _playingSounds = new();
        // 단발 사운드는 이름별로 살아있는 AudioSource들을 추적해 개수를 제한한다(가장 오래된 것부터 회수).
        private readonly Dictionary<string, List<AudioSource>> _oneShots = new();
        private void Awake()
        {
            if (_soundListSo == null)
            {
                Debug.Assert(_soundListSo != null,$"SoundListSo asset is null");
            }
            if (_mixer == null)
            {
                Debug.LogError("AudioMixer가 할당되지 않았습니다. SoundManager를 사용하기 전에 할당해주세요.");
            }
        }

        private void Start()
        {
            // AudioMixer.SetFloat은 Awake 타이밍엔 믹서가 아직 완전히 로드되지 않아 무시된다.
            // (설정창을 열 때 VolumeSlider가 다시 적용해야 비로소 들리는 증상의 원인)
            // 한 프레임 뒤에 적용해야 저장된 볼륨이 게임 시작부터 반영된다.
            StartCoroutine(ApplySavedVolumesDelayed());
        }

        private IEnumerator ApplySavedVolumesDelayed()
        {
            yield return null;
            ApplySavedVolumes();
        }

        private void ApplySavedVolumes()
        {
            if (_mixer == null) return;
            VolumeSettings.Apply(_mixer, VolumeSettings.MasterParam, VolumeSettings.Get(VolumeSettings.MasterParam));
            VolumeSettings.Apply(_mixer, VolumeSettings.BgmParam, VolumeSettings.Get(VolumeSettings.BgmParam));
            VolumeSettings.Apply(_mixer, VolumeSettings.SfxParam, VolumeSettings.Get(VolumeSettings.SfxParam));
        }
        public void PlaySound(string soundName) => PlaySoundInternal(soundName, null);

        public void PlaySound(string soundName, Transform transform) => PlaySoundInternal(soundName, transform);

        private void PlaySoundInternal(string soundName, Transform at)
        {
            if (_soundListSo == null ||
                !_soundListSo.SoundsDictionary.TryGetValue(soundName, out SoundSo so) || so == null)
            {
                Debug.LogWarning($"사운드 '{soundName}'를 SoundList에서 찾을 수 없습니다.");
                return;
            }
            if (so.clip == null)
            {
                Debug.LogWarning($"사운드 '{soundName}'에 AudioClip이 없습니다.");
                return;
            }

            // 루프 사운드는 이름당 하나만 유지(중복 루프 방지)하므로 먼저 정리하고 딕셔너리로 추적한다.
            if (so.loop)
                StopSound(soundName);
            else
                // 단발 SFX는 서로 끊지 않고 겹쳐 재생되되, 같은 소리가 무한정 쌓이지 않게 개수를 제한한다.
                // (연사 시 수십 개가 쌓여 오디오 보이스 한도를 넘기면 새 발사음이 안 들리는 문제 방지)
                EnforceOneShotCap(soundName);

            GameObject obj = new GameObject(soundName + " Sound");
            if (at != null)
                obj.transform.position = at.position;

            AudioSource source = obj.AddComponent<AudioSource>();

            if (so.loop)
            {
                _playingSounds[soundName] = source;
            }
            else
            {
                if (!_oneShots.TryGetValue(soundName, out var list))
                    _oneShots[soundName] = list = new List<AudioSource>();
                list.Add(source);
            }

            AssignMixerGroup(source, so);
            SetAudio(source, so);
        }

        // 같은 이름 단발 사운드가 한계 개수에 도달하면, 가장 오래된 것부터 정리해 새 소리가 항상 재생되게 한다.
        private void EnforceOneShotCap(string soundName)
        {
            if (!_oneShots.TryGetValue(soundName, out var list)) return;

            list.RemoveAll(s => s == null);

            int cap = Mathf.Max(1, maxConcurrentPerSound);
            while (list.Count >= cap)
            {
                AudioSource oldest = list[0];
                list.RemoveAt(0);
                if (oldest != null) Destroy(oldest.gameObject);
            }
        }

        private void AssignMixerGroup(AudioSource source, SoundSo so)
        {
            if (_mixer == null)
            {
                Debug.LogWarning("Mixer가 할당되지 않았습니다. SoundManager를 사용하기 전에 할당해주세요.");
                return;
            }

            string groupName = so.soundType switch
            {
                SoundType.SFX => "SFX",
                SoundType.BGM => "BGM",
                _ => "Master"
            };

            var groups = _mixer.FindMatchingGroups(groupName);
            if (groups.Length > 0)
                source.outputAudioMixerGroup = groups[0];
        }

        private void SetAudio(AudioSource source,SoundSo sounds)
        {
            source.clip = sounds.clip;
            source.loop = sounds.loop;
            source.priority = sounds.Priority;
            source.volume = sounds.volume;
            source.panStereo = sounds.stereoPan;
            source.spatialBlend = sounds.SpatialBlend;

            // RandomPitch면 재생할 때마다 무작위 피치를 '지역 변수'로 계산한다.
            // (이전엔 sounds.pitch(=SO 에셋 필드)를 직접 덮어써서 에셋이 영구 변형됐고,
            //  게다가 source.pitch를 먼저 대입한 뒤라 정작 이번 재생엔 적용되지도 않았다.)
            float pitch = sounds.RandomPitch ? Random.Range(sounds.MinPitch, sounds.MaxPitch) : sounds.pitch;
            source.pitch = pitch;

            // 역재생(음수 피치)은 클립 끝에서 시작해야 소리가 난다.
            if (pitch < 0f)
                source.time = Mathf.Max(0f, sounds.clip.length - 0.01f);

            source.Play();

            if (!sounds.loop)
            {
                // 실제 재생 길이는 피치 배속에 반비례한다(피치 0 방지).
                float playLength = sounds.clip.length / Mathf.Max(0.01f, Mathf.Abs(pitch));
                StartCoroutine(DestroyCo(playLength, source));
            }
        }

        IEnumerator DestroyCo(float endTime, AudioSource source)
        {
            yield return new WaitForSecondsRealtime(endTime);

            // 재생 끝난 사운드를 딕셔너리에서도 제거해야 죽은 참조가 안 남는다(StopSound 크래시 방지).
            if (source != null)
            {
                string key = null;
                foreach (var kv in _playingSounds)
                    if (kv.Value == source) { key = kv.Key; break; }
                if (key != null) _playingSounds.Remove(key);

                // 단발 추적 목록에서도 제거
                foreach (var kv in _oneShots)
                    if (kv.Value.Remove(source)) break;

                Destroy(source.gameObject);
            }
        }

        public void StopSound(string soundName)
        {
            if (_playingSounds.TryGetValue(soundName, out AudioSource obj))
            {
                _playingSounds.Remove(soundName);
                // DestroyCo로 이미 파괴됐을 수 있으니 null 체크(파괴된 객체 접근 시 예외 방지).
                if (obj != null)
                {
                    obj.Stop();
                    Destroy(obj.gameObject);
                }
            }
        }
    }

    public enum SoundType
    {
        BGM,
        SFX
    }
}