using System;
using UnityEngine;

namespace _02_Scripts.Chip.WallRide
{
    [CreateAssetMenu(fileName = "WallRideChipDataSO", menuName = "Chip/WallRideChipDataSO")]
    public class WallRideChipDataSO : ChipDataSO
    {
        [Serializable]
        public class WallRideLevelData
        {
            public float WallRideSpeed;
            public float WallJumpSideForce;
            public float WallJumpUpForce;
            public float WallReleaseForce;
        }

        public LayerMask WallLayer;
        [field: SerializeField] public WallRideLevelData[] LevelData { get; private set; }
    }
}
