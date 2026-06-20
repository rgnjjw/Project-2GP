using UnityEngine;

namespace _02_Scripts.Map
{
    [CreateAssetMenu(fileName = "WaveDataSO", menuName = "Map/WaveDataSO")]
    public class WaveDataSO : ScriptableObject
    {
        [Tooltip("이 웨이브의 상대 크기 배수. 1=기준, 0.5=절반, 2=두 배. " +
                 "실제 적 수는 레벨 기반 기준 수에 이 배수를 곱해 정해진다(한 맵 안에서 웨이브마다 크기 차등).")]
        [Min(0f)] public float sizeMultiplier = 1f;
    }
}
