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
        public void PlaySound(string soundName)
        {
            GameObject obj = new GameObject();
            obj.name = soundName + " Sound";
            AudioSource source = obj.AddComponent<AudioSource>();
            SoundSo so = _soundListSo.SoundsDictionary[soundName];
            _playingSounds[soundName] = source;
            if (_mixer == null)
            {
                Debug.LogWarning("Mixer가 할당되지 않았습니다. SoundManager를 사용하기 전에 할당해주세요.");
                SetAudio(source,so);
                return;
            }
            if(so.soundType == SoundType.SFX)
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];
            else if(so.soundType == SoundType.BGM)
            {
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("BGM")[0];
            }
            else
            {
                Debug.LogWarning("Type이 없습니다");
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master")[0];
            }
            SetAudio(source,so);
        
        }
        public void PlaySound(string soundName, Transform transform)
        {
            GameObject obj = new GameObject();
            obj.name = soundName + " Sound";
            obj.transform.position = transform.position;
            AudioSource source = obj.AddComponent<AudioSource>();
            SoundSo so = _soundListSo.SoundsDictionary[soundName];
            _playingSounds[soundName] = source;
            if (_mixer == null)
            {
                Debug.LogWarning("Mixer가 할당되지 않았습니다. SoundManager를 사용하기 전에 할당해주세요.");
                SetAudio(source,so);
                return;
            }
            if(so.soundType == SoundType.SFX)
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];
            else if(so.soundType == SoundType.BGM)
            {
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("BGM")[0];
            }
            else
            {
                Debug.LogWarning("Type이 없습니다");
                source.outputAudioMixerGroup = _mixer.FindMatchingGroups("Master")[0];
            }
            SetAudio(source,so);
        
        }

        private void SetAudio(AudioSource source,SoundSo sounds)
        {
            source.clip = sounds.clip;
            source.loop = sounds.loop;
            source.priority = sounds.Priority;
            source.volume = sounds.volume;
            source.pitch = sounds.pitch;
            source.panStereo = sounds.stereoPan;
            source.spatialBlend = sounds.SpatialBlend;
            if (sounds.RandomPitch)
            {
                sounds.pitch = Random.Range(sounds.MinPitch, sounds.MaxPitch);
            }
            if (sounds.pitch < 0)
            {
                source.time = 1;
            }
            source.Play();
            if (!sounds.loop) { StartCoroutine(DestroyCo(source.clip.length,source.gameObject)); }

        }

        IEnumerator DestroyCo(float endTime,GameObject obj)
        {
            yield return new WaitForSecondsRealtime(endTime);
            Destroy(obj);
        }

        public void StopSound(string soundName)
        {
            if (_playingSounds.TryGetValue(soundName, out AudioSource obj))
            {
                obj.Stop();
                _playingSounds.Remove(soundName);
                Destroy(obj.gameObject);
            }
        }
    }

    public enum SoundType
    {
        BGM,
        SFX
    }
}