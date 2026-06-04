using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Weapon
{
    public class WeaponManager : MonoBehaviour, IModule
    {
        [SerializeField] private Gun.Gun[] weapons;
        public Gun.Gun CurrentWeapon { get; private set; }
        private PlayerInputSO _playerInput;
        private int _currentIndex;

        public void Initialize(ModuleOwner owner)
        {
            if (owner is Player.Player player)
            {
                _playerInput = player.PlayerInputSO;
                SwapWeapon(0);
            }
        }

        
        //나중에 인풋이랑 연결하기
        private void SwapWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;
        
            _currentIndex = index;
            CurrentWeapon = weapons[_currentIndex];
            CurrentWeapon.Equip();
        }

        private void SwapNext() => SwapWeapon((_currentIndex + 1) % weapons.Length);
        private void SwapPrev() => SwapWeapon((_currentIndex - 1 + weapons.Length) % weapons.Length);
    }
}