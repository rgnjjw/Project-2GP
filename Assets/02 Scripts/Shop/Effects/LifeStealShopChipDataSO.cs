using System;
using UnityEngine;

namespace _02_Scripts.Shop.Effects
{
    [CreateAssetMenu(menuName = "Shop/Effects/LifeStealShopChipDataSO", fileName = "Chip_LifeSteal")]
    public class LifeStealShopChipDataSO : ShopChipDataSO
    {
        [field: SerializeField] public LevelData[] LevelStats { get; private set; }

        [Serializable]
        public class LevelData
        {
            public float StealRate;
        }

        public float GetStealRate(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, LevelStats.Length - 1);
            return LevelStats[idx].StealRate;
        }
    }
}
