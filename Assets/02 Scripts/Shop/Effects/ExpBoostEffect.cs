using _02_Scripts.UI;

namespace _02_Scripts.Shop.Effects
{
    [ShopChip("ExpBoost")]
    public class ExpBoostEffect : IShopPassiveEffect
    {
        private float _currentBonus;

        public void OnEquip(Player.Player player, ShopChipDataSO chipData)
        {
            var data = (ExpBoostShopChipDataSO)chipData;
            _currentBonus = data.GetBonus(1);
            LevelManager.Instance.ExpMultiplier += _currentBonus;
        }

        public void OnUnequip(Player.Player player)
        {
            LevelManager.Instance.ExpMultiplier -= _currentBonus;
            _currentBonus = 0f;
        }

        public void OnLevelUp(int newLevel, ShopChipDataSO chipData)
        {
            LevelManager.Instance.ExpMultiplier -= _currentBonus;
            var data = (ExpBoostShopChipDataSO)chipData;
            _currentBonus = data.GetBonus(newLevel);
            LevelManager.Instance.ExpMultiplier += _currentBonus;
        }
    }
}
