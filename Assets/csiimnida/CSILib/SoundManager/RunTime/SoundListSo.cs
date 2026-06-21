using System.Collections.Generic;
using CSILib.SoundManager.RunTime;
using UnityEngine;

namespace csiimnida.CSILib.SoundManager.RunTime
{
    [CreateAssetMenu(fileName = "SoundListSO", menuName = "SO/Sound/SoundListSO")]
    public class SoundListSo : ScriptableObject
    {
        [SerializeField] private List<SoundSo> Sounds = new List<SoundSo>();

        public Dictionary<string, SoundSo> SoundsDictionary;

        private void OnEnable()
        {
			if(Sounds == null)
				return;
            SoundsDictionary = new Dictionary<string, SoundSo>();
            foreach (SoundSo soundSo in Sounds)
            {
                if (soundSo == null || string.IsNullOrEmpty(soundSo.soundName))
                    continue;
                
                SoundsDictionary[soundSo.soundName] = soundSo;
            }
        }
        public void AddSound(SoundSo soundSo)
        {
            if (soundSo != null && !string.IsNullOrEmpty(soundSo.soundName))
            {
                Sounds.Add(soundSo);
                SoundsDictionary[soundSo.soundName] = soundSo;
            }
        }

        public List<SoundSo> GetSoundList() => Sounds;

        public void RemoveSound(SoundSo so)
        {
            if (so != null)
            {
                Sounds.Remove(so);
            }
        }
    }
}
