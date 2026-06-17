using System;
using UnityEngine;

namespace _02_Scripts.Shop
{
    [CreateAssetMenu(menuName = "Shop/WeaponShopChipDataSO", fileName = "New WeaponShopChipData")]
    public class WeaponShopChipDataSO : ShopChipDataSO
    {
        [field: SerializeField] public int WeaponIndex { get; private set; }
        [field: SerializeField] public WeaponLevelData[] WeaponLevelStats { get; private set; }

        [Serializable]
        public class WeaponLevelData
        {
            public int BonusDamage;
            public float BonusFireRate;
        }
    }
}
