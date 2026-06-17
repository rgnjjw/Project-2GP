using System;
using UnityEngine;

namespace _02_Scripts.Shop.Effects
{
    [CreateAssetMenu(menuName = "Shop/Effects/ExpBoostShopChipDataSO", fileName = "Chip_ExpBoost")]
    public class ExpBoostShopChipDataSO : ShopChipDataSO
    {
        [field: SerializeField] public LevelData[] LevelStats { get; private set; }

        [Serializable]
        public class LevelData
        {
            public float BonusMultiplier;
        }

        public float GetBonus(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, LevelStats.Length - 1);
            return LevelStats[idx].BonusMultiplier;
        }
    }
}
