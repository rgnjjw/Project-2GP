using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.Chip
{
    public class ChipInputHandler : MonoBehaviour,IModule
    {
        private Dictionary<int, IChip> _chipDictionary;
        public void Initialize(ModuleOwner owner)
        {
            _chipDictionary = new Dictionary<int, IChip>();
            
            if (owner is Player.Player player)
            {
                player.PlayerInputSO.OnChipInput += HandleChipInput;
            }
        }

        private void HandleChipInput(int index, InputAction.CallbackContext context)
        {
            foreach (var chip in _chipDictionary)
            {
                if (chip.Key == index)
                {
                    chip.Value.Execute(context);
                }
            }
        }
        
        public void Equip(IChip chip) => _chipDictionary.Add((int)chip.ChipType, chip);
        
        public void Unequip(ChipEnum chipType) => _chipDictionary.Remove((int)chipType);
    }
}