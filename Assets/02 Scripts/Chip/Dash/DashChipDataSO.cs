using System;
using UnityEngine;

namespace _02_Scripts.Chip.Dash
{
    [CreateAssetMenu(fileName = "DashChipDataSO", menuName = "Chip/DashChipDataSO")]
    public class DashChipDataSO : ChipDataSO
    {
        [Serializable]
        public class DashLevelData
        {
            public int MaxDashCount;
            public float DashSpeed;
            public float DashDuration;
            public float RechargeTime;
        }

        [field: SerializeField] public DashLevelData[] LevelData { get; private set; }
    }
}
