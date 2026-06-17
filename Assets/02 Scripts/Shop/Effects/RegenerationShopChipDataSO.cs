using System;
using UnityEngine;

namespace _02_Scripts.Shop.Effects
{
    [CreateAssetMenu(menuName = "Shop/Effects/RegenerationShopChipDataSO", fileName = "Chip_Regeneration")]
    public class RegenerationShopChipDataSO : ShopChipDataSO
    {
        [field: SerializeField] public LevelData[] LevelStats { get; private set; }

        [Serializable]
        public class LevelData
        {
            public int HealPerSecond;
        }

        public int GetHealPerSecond(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, LevelStats.Length - 1);
            return LevelStats[idx].HealPerSecond;
        }
    }
}
