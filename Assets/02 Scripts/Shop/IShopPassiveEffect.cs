namespace _02_Scripts.Shop
{
    public interface IShopPassiveEffect
    {
        void OnEquip(Player.Player player, ShopChipDataSO chipData);
        void OnUnequip(Player.Player player);
        void OnLevelUp(int newLevel, ShopChipDataSO chipData);
    }
}
