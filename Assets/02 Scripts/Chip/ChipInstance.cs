namespace _02_Scripts.Chip
{
    public class ChipInstance
    {
        public ChipDataSO Data;
        public int  CurrentLevel = 1;
        public bool IsEquipped;

        private IChip _effect;

        public ChipInstance(ChipDataSO data)
        {
            Data = data;
        }

        public IChip GetEffect()
        {
            _effect ??= ChipFactory.Create(Data.ChipId);
            return _effect;
        }

        public void LevelUp()
        {
            if (CurrentLevel < Data.MaxLevel)
                CurrentLevel++;
        }
    }
}