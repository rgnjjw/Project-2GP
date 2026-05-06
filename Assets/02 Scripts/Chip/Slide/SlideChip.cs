using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Chip.Slide
{
    [Chip("Slide")]
    public class SlideChip : IChip
    {
        private PlayerSlider _playerSlider;

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            _playerSlider = player.GetModule<PlayerSlider>();
            if (_playerSlider == null)
            {
                Debug.LogError("[SlideChip] PlayerSlider module not found on player!");
                return;
            }
            ApplyLevelStats(chip);
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            _playerSlider?.Disable();
        }

        public void OnLevelUp(ChipInstance chip) => ApplyLevelStats(chip);

        private void ApplyLevelStats(ChipInstance chip)
        {
            if (chip.Data is SlideChipDataSO data)
                _playerSlider.Enable(data.LevelData[chip.CurrentLevel - 1].SlideBoostSpeed);
        }
    }
}
