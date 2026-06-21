using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;

namespace _02_Scripts.Sound
{
    /// <summary>
    /// 씬의 아무 오브젝트에나 붙여두면 시작 시 지정한 BGM을 한 번 재생한다.
    /// SoundSo의 loop가 켜져 있어야 계속 반복된다(MainSceneBGM은 loop=1).
    /// </summary>
    public class BgmStarter : MonoBehaviour
    {
        [Tooltip("SoundManager에 등록된 SoundSo의 soundName")]
        [SerializeField] private string bgmName = "MainSceneBGM";

        private void Start()
        {
            if (string.IsNullOrEmpty(bgmName)) return;
            SoundManager.Instance.PlaySound(bgmName);
        }
    }
}
