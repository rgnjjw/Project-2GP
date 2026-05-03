namespace _02_Scripts.Chip
{
    public interface IChip
    {
        void OnEquip(ChipInstance chip, Player.Player player);
        void OnUnequip(ChipInstance chip, Player.Player player);
        void OnLevelUp(ChipInstance chip);
    }
}