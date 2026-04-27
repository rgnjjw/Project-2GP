using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.Chip
{
    public class ChipManager : MonoBehaviour,IModule
    {
        public Dictionary<int, IChip> ChipDictionary {get; private set; }

        public void Initialize(ModuleOwner owner)
        {
            ChipDictionary = new Dictionary<int, IChip>();
        }
        
        
        

    }
}