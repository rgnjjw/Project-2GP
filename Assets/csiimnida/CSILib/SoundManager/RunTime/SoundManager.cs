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

        private Dictionary<string, AudioSource> _playingSounds = new();
        // 단발 사운드는 이름별 '영구 AudioSource' 하나로 PlayOneShot한다.
        // → 발사할 때마다 GameObject를 새로 만들지 않아 생성 지연/싱크 밀림이 없고,
        //   PlayOneShot이 겹침을 알아서 처리해 연사에도 끊기지 않는다.
        private readonly Dictionary<string, AudioSource> _oneShotPlayers = new();
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

            if (so.loop)
            {
                // 루프 사운드: 이름당 하나만 유지(중복 루프 방지). 전용 GameObject로 재생 + 추적.
                StopSound(soundName);

                GameObject obj = new GameObject(soundName + " Sound");
                if (at != null) obj.transform.position = at.position;

                AudioSource source = obj.AddComponent<AudioSource>();
                _playingSounds[soundName] = source;
                AssignMixerGroup(source, so);
                SetAudio(source, so);
                return;
            }

            // 단발(one-shot): 이름별 영구 소스로 PlayOneShot.
            // 매 발사 GameObject 생성이 없어 지연/싱크 밀림이 없고, 겹침도 자연스럽게 처리된다.
            AudioSource player = GetOneShotPlayer(soundName);
            if (at != null) player.transform.position = at.position;

            AssignMixerGroup(player, so);
            player.priority = so.Priority;
            player.volume = so.volume;
            player.panStereo = so.stereoPan;
            player.spatialBlend = so.SpatialBlend;
            player.pitch = so.RandomPitch ? Random.Range(so.MinPitch, so.MaxPitch) : so.pitch;

            player.PlayOneShot(so.clip);
        }

        // 단발 사운드용 영구 AudioSource를 이름별로 하나 만들어 재사용한다.
        private AudioSource GetOneShotPlayer(string soundName)
        {
            if (_oneShotPlayers.TryGetValue(soundName, out AudioSource src) && src != null)
                return src;

            GameObject obj = new GameObject($"OneShot_{soundName}");
            obj.transform.SetParent(transform, false);
            src = obj.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            _oneShotPlayers[soundName] = src;
            return src;
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