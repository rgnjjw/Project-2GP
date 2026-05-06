using System;
using UnityEngine;

namespace _02_Scripts.Chip.Slide
{
    [CreateAssetMenu(fileName = "SlideChipDataSO", menuName = "Chip/SlideChipDataSO")]
    public class SlideChipDataSO : ChipDataSO
    {
        [Serializable]
        public class SlideLevelData
        {
            public float SlideBoostSpeed;
        }

        [field: SerializeField] public SlideLevelData[] LevelData { get; private set; }
    }
}
