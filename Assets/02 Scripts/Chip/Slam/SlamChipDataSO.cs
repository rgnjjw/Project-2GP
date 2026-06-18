using System;
using UnityEngine;

namespace _02_Scripts.Chip.Slam
{
    [CreateAssetMenu(fileName = "SlamChipDataSO", menuName = "Chip/SlamChipDataSO")]
    public class SlamChipDataSO : ChipDataSO
    {
        [Serializable]
        public class SlamLevelData
        {
            public int Damage;
            public float DamageRadius;
        }

        [field: SerializeField] public SlamLevelData[] LevelData { get; private set; }
        [field: SerializeField] public float SlamDownSpeed { get; private set; } = 30f;
        // [field: SerializeField] public GameObject DustEffectPrefab { get; private set; }
        [field: SerializeField] public float ShakeStrength { get; private set; } = 5f;
        [field: SerializeField] public Vector3 ShakeDirection { get; private set; } = Vector3.down;
        [field: SerializeField] public LayerMask EnemyLayer { get; private set; }
    }
}
