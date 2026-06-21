using UnityEngine;
using UnityEngine.Audio;

namespace csiimnida.CSILib.SoundManager.RunTime
{
    /// <summary>
    /// 볼륨 값(0~1)을 PlayerPrefs에 저장하고 AudioMixer에 적용하는 공용 유틸.
    /// 슬라이더(VolumeSlider)와 SoundManager가 같은 키/변환식을 공유한다.
    /// </summary>
    public static class VolumeSettings
    {
        public const string MasterParam = "Master";
        public const string BgmParam = "BGM";
        public const string SfxParam = "SFX";

        private static string KeyOf(string param) => "vol_" + param;

        /// 저장된 볼륨(0~1)을 읽는다. 없으면 1(최대).
        public static float Get(string param) =>
            Mathf.Clamp01(PlayerPrefs.GetFloat(KeyOf(param), 1f));

        public static void Save(string param, float value01)
        {
            PlayerPrefs.SetFloat(KeyOf(param), Mathf.Clamp01(value01));
            PlayerPrefs.Save();
        }

        /// 0~1 값을 데시벨로 변환해 믹서에 적용한다.
        public static void Apply(AudioMixer mixer, string param, float value01)
        {
            if (mixer == null) return;
            value01 = Mathf.Clamp01(value01);
            float dB = value01 <= 0.0001f ? -80f : Mathf.Log10(value01) * 20f;
            mixer.SetFloat(param, dB);
        }

        public static void SaveAndApply(AudioMixer mixer, string param, float value01)
        {
            Save(param, value01);
            Apply(mixer, param, value01);
        }
    }
}
