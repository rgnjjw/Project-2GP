using System;
using System.Linq;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Player
{
    [Serializable]
    public enum PlayerVisualState
    {
        ALL,
        PISTOL,
        SHOTGUN,
        MACHINEGUN
    }

    public class PlayerVisualController : MonoBehaviour ,IModule //플레이어의 비쥬얼을 컨트롤 하는놈
    {
        public PlayerRenderer CurrentVisual {get; private set; }
        private PlayerRenderer[] _playerVisuals;
        public void Initialize(ModuleOwner owner)
        {
            if (owner is Player)
            {
                _playerVisuals = owner.GetModules<PlayerRenderer>().ToArray();
                ChangeVisual(PlayerVisualState.PISTOL);
            }
        }

        public void ChangeVisual(PlayerVisualState newVisual)
        {
            foreach (var visual in _playerVisuals)
            {
                if (visual.visualState == newVisual)
                {
                    CurrentVisual = visual;
                    visual.gameObject.SetActive(true);
                }
                else
                {
                    visual.gameObject.SetActive(false);
                }
            }
        }

    }
}