using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    /// <summary>
    /// Slider 하나를 특정 볼륨 채널(Master/BGM/SFX)에 연결한다.
    /// 켜질 때 저장된 값을 불러와 슬라이더와 믹서에 반영하고,
    /// 슬라이더를 움직이면 PlayerPrefs에 저장 + 믹서에 적용한다.
    /// 슬라이더 범위(min~max)는 자유 — 내부에서 0~1로 정규화한다.
    /// </summary>
    [RequireComponent(typeof(Slider))]
    public class VolumeSlider : MonoBehaviour
    {
        public enum Channel
        {
            Master,
            BGM,
            SFX
        }

        [SerializeField] private Channel channel;
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private Slider slider;

        // enum 이름이 믹서 노출 파라미터 이름과 동일("Master"/"BGM"/"SFX")
        private string Param => channel.ToString();

        private void Awake()
        {
            if (slider == null) slider = GetComponent<Slider>();
        }

        private void OnEnable()
        {
            float value01 = VolumeSettings.Get(Param);
            slider.SetValueWithoutNotify(Mathf.Lerp(slider.minValue, slider.maxValue, value01));
            VolumeSettings.Apply(mixer, Param, value01);
            slider.onValueChanged.AddListener(OnSliderChanged);
        }

        private void OnDisable()
        {
            slider.onValueChanged.RemoveListener(OnSliderChanged);
        }

        private void OnSliderChanged(float raw)
        {
            float value01 = Mathf.InverseLerp(slider.minValue, slider.maxValue, raw);
            VolumeSettings.SaveAndApply(mixer, Param, value01);
        }
    }
}
