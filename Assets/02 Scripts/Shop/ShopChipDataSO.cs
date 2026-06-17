using UnityEngine;

namespace _02_Scripts.Shop
{
    [CreateAssetMenu(menuName = "Shop/ShopChipDataSO", fileName = "New ShopChipData")]
    public class ShopChipDataSO : ScriptableObject
    {
        [field: SerializeField] public string ChipId { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public ChipCategory Category { get; private set; }
        [field: SerializeField] public int MaxLevel { get; private set; } = 1;
        [field: SerializeField] public int BasePrice { get; private set; }
        [field: SerializeField] public int[] UpgradePrices { get; private set; }
        [field: SerializeField] public string[] LevelDescriptions { get; private set; }

        public int GetUpgradePrice(int currentLevel)
        {
            int idx = currentLevel - 1;
            if (UpgradePrices == null || idx < 0 || idx >= UpgradePrices.Length) return 0;
            return UpgradePrices[idx];
        }

        public int GetSellRefund(float refundRate) => Mathf.FloorToInt(BasePrice * refundRate);
    }
}
