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
        [field: SerializeField] public GameObject DustEffectPrefab { get; private set; }
        [field: SerializeField] public float ShakeDuration { get; private set; } = 0.3f;
        [field: SerializeField] public float ShakeStrength { get; private set; } = 0.4f;
        [field: SerializeField] public int ShakeVibrato { get; private set; } = 20;
        [field: SerializeField] public LayerMask EnemyLayer { get; private set; }
    }
}
