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
            // 효과 생성 책임은 데이터(ChipDataSO)가 가진다. ChipInstance는 구체 타입을 알 필요가 없다.
            _effect ??= Data.CreateEffect();
            return _effect;
        }

        public void LevelUp()
        {
            if (CurrentLevel < Data.MaxLevel)
                CurrentLevel++;
        }
    }
}