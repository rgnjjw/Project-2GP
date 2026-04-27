namespace _02_Scripts.Chip
{
    public class AbstractChip : IChip
    {
        public ChipEnum ChipType { get; }
        public string Name { get; }
        public string Description { get; }
    }
}