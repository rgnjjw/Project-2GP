using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Chip
{
    public class ChipManager : MonoBehaviour,IModule
    {
        public int MaxSlot;
        private Player.Player _player;
        private List<ChipInstance> _equippedChipList;
        public void Initialize(ModuleOwner owner)
        {
            _equippedChipList =  new List<ChipInstance>();
            if (owner is Player.Player player)
            {
                _player = player;
            }
        }

        public bool EquipChip(ChipInstance chip)
        {
            if(_equippedChipList.Count >= MaxSlot) return false;

            var effect = chip.GetEffect();//효과 생성
            effect.OnEquip(chip, _player);

            chip.IsEquipped = true;
            _equippedChipList.Add(chip);
            return true;
        }

        public void UnequipChip(ChipInstance chip)
        {
            if (!chip.IsEquipped) return;//장착이 안된 칩이라면
            if (!_equippedChipList.Contains(chip)) return;//리스트의 없는 칩이라면
            
            var effect = chip.GetEffect();
            effect.OnUnequip(chip, _player);
            
            chip.IsEquipped = false;
            _equippedChipList.Remove(chip);
        }

        public void ChipLevelUp(ChipInstance chip)
        {
            if (!chip.IsEquipped) return;
            if(!_equippedChipList.Contains(chip)) return;
            
            chip.LevelUp();
            chip.GetEffect().OnLevelUp(chip);
        }
        public IReadOnlyList<ChipInstance> GetEquippedChips() => _equippedChipList;
    }
}