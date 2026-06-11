 using System;
using UnityEngine;

namespace _02_Scripts.Chip.Run
{
    [CreateAssetMenu(fileName = "RunChipDataSO", menuName = "Chip/RunChipDataSO")]
    public class RunChipDataSO : ChipDataSO
    {
        [Serializable]
        public class RunLevelData
        {
            public float StaminaDuration;
            public float RunSpeedMultiplier;
            [Tooltip("달리기 종료 후 스태미너 회복 시작까지 대기 시간 (초)")]
            public float RechargeDelay;
            [Tooltip("초당 스태미너 회복량")]
            public float RechargeRate;
        }

        [field: SerializeField] public RunLevelData[] LevelData { get; private set; }
    }
}
