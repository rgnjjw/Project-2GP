using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Chip
{
    public class ChipManager : MonoBehaviour,IModule
    {
        private Dictionary<int,IChip> _chipDictionary;
        
        public void Initialize(ModuleOwner owner)
        {
            _chipDictionary = new Dictionary<int, IChip>();
        }
        
        public void Equip(IChip chip)
        {
            _chipDictionary.Add((int)chip.ChipType, chip);
        }

        public void Unequip(ChipEnum chipType)
        {
            _chipDictionary.Remove((int)chipType);
        }

    }
}