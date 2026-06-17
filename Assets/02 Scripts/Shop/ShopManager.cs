using System;
using _02_Scripts.Core.Utility;
using _02_Scripts.Manager;
using UnityEngine;

namespace _02_Scripts.Shop
{
    public class ShopManager : MonoSingleton<ShopManager>
    {
        [SerializeField] private float sellRefundRate = 0.7f;

        public event Action<ShopChipDataSO> OnPurchaseSuccess;
        public event Action<ShopChipDataSO> OnPurchaseFailed;
        public event Action<ShopChipDataSO, int> OnUpgradeSuccess;
        public event Action<ShopChipDataSO> OnUpgradeFailed;
        public event Action<ShopChipDataSO, int> OnSellSuccess;
        public event Action<int> OnWeaponUnlocked;

        public bool TryPurchase(ShopChipDataSO chip)
        {
            var inventory = PlayerChipInventory.Instance;

            if (chip.Category == ChipCategory.Weapon && inventory.IsOwned(chip.ChipId))
            {
                OnPurchaseFailed?.Invoke(chip);
                return false;
            }

            if (!CurrencyManager.Instance.TrySpend(chip.BasePrice))
            {
                OnPurchaseFailed?.Invoke(chip);
                return false;
            }

            inventory.AddChip(chip);

            if (chip.Category == ChipCategory.Passive)
                inventory.TryAutoEquipPassive(chip);

            if (chip is WeaponShopChipDataSO weaponChip)
                OnWeaponUnlocked?.Invoke(weaponChip.WeaponIndex);

            OnPurchaseSuccess?.Invoke(chip);
            return true;
        }

        public bool TryUpgrade(ShopChipDataSO chip)
        {
            var inventory = PlayerChipInventory.Instance;

            if (!inventory.IsOwned(chip.ChipId) || chip.Category != ChipCategory.Passive)
            {
                OnUpgradeFailed?.Invoke(chip);
                return false;
            }

            int currentLevel = inventory.GetCurrentLevel(chip.ChipId);
            if (currentLevel >= chip.MaxLevel)
            {
                OnUpgradeFailed?.Invoke(chip);
                return false;
            }

            int upgradePrice = chip.GetUpgradePrice(currentLevel);
            if (!CurrencyManager.Instance.TrySpend(upgradePrice))
            {
                OnUpgradeFailed?.Invoke(chip);
                return false;
            }

            inventory.UpgradeChip(chip);
            OnUpgradeSuccess?.Invoke(chip, inventory.GetCurrentLevel(chip.ChipId));
            return true;
        }

        public bool TrySell(ShopChipDataSO chip)
        {
            if (chip.Category == ChipCategory.Weapon)
                return false;

            var inventory = PlayerChipInventory.Instance;

            if (!inventory.IsOwned(chip.ChipId))
                return false;

            int refund = chip.GetSellRefund(sellRefundRate);
            inventory.RemoveChip(chip);
            CurrencyManager.Instance.AddCurrency(refund);

            OnSellSuccess?.Invoke(chip, refund);
            return true;
        }
    }
}
