using System;
using UnityEngine;

namespace _02_Scripts.Shop.Effects
{
    [CreateAssetMenu(menuName = "Shop/Effects/InvincibilityShopChipDataSO", fileName = "Chip_Invincibility")]
    public class InvincibilityShopChipDataSO : ShopChipDataSO
    {
        [field: SerializeField] public float HpThreshold { get; private set; } = 0.3f;
        [field: SerializeField] public float InvincibleDuration { get; private set; } = 3f;
        [field: SerializeField] public LevelData[] LevelStats { get; private set; }

        [Serializable]
        public class LevelData
        {
            public float Cooldown;
        }

        public float GetCooldown(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, LevelStats.Length - 1);
            return LevelStats[idx].Cooldown;
        }
    }
}
