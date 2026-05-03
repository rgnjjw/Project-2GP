using System;
using UnityEngine;

namespace _02_Scripts.Chip.Jump
{
    [CreateAssetMenu(fileName = "JumpChipDataSO", menuName = "Chip/JumpChipDataSO")]
    public class JumpChipDataSO : ChipDataSO
    {
        [Serializable]
        public class JumpLevelData
        {
            public int JumpPower;
            public int MaxJumpCount;
        }

        [field: SerializeField] public JumpLevelData[] LevelData { get; private set; }
    }
}