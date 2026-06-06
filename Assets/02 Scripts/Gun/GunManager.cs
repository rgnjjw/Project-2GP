using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class GunManager : MonoBehaviour, IModule,IAfterInitModule
    {
        [SerializeField] private Gun[] weapons;
        private Gun _currentWeapon;
        private int _currentIndex;
        private PlayerInputSO _playerInput;
        private PlayerVisualController _playerVisualController;

        public void Initialize(ModuleOwner owner)
        {
            if (owner is Player.Player player)
            {
                _playerInput = player.PlayerInputSO;
                _playerVisualController = player.GetModule<PlayerVisualController>();
                _playerInput.OnScrollWeaponInput += OnScroll;
                _playerInput.OnWeapon1Pressed += OnWeapon1;
                _playerInput.OnWeapon2Pressed += OnWeapon2;
            }
        }

        public void AfterInit()
        {
            SwapWeapon(0); 
        }

        private void SwapWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;

            _currentIndex = index;
            _currentWeapon = weapons[_currentIndex];
    
            _playerVisualController.ChangeVisual(index switch
            {
                0 => PlayerVisualState.PISTOL,
                1 => PlayerVisualState.SHOTGUN,
                _ => PlayerVisualState.ALL
            });
    
            _currentWeapon.Equip();
        }

        private void OnScroll(float scroll)
        {
            if (scroll > 0) SwapNext();
            else if (scroll < 0) SwapPrev();
        }

        private void OnWeapon1() => SwapWeapon(0);
        private void OnWeapon2() => SwapWeapon(1);

        public void Fire() => _currentWeapon.Fire();

        private void SwapNext() => SwapWeapon((_currentIndex + 1) % weapons.Length);
        private void SwapPrev() => SwapWeapon((_currentIndex - 1 + weapons.Length) % weapons.Length);

        private void OnDestroy()
        {
            _playerInput.OnScrollWeaponInput -= OnScroll;
            _playerInput.OnWeapon1Pressed -= OnWeapon1;
            _playerInput.OnWeapon2Pressed -= OnWeapon2;
        }
    }
}