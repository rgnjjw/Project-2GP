using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Player;
using _02_Scripts.UI;
using UnityEngine;

namespace _02_Scripts.Manager
{
    public class GunManager : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private Gun.Gun[] weapons;
        [SerializeField] private CurrentGunUI currentGunUI;
        private Gun.Gun _currentWeapon;
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
                _playerInput.OnWeapon3Pressed += OnWeapon3;
                _playerInput.OnFireKeyPressed += OnFirePressed;
            }
        }

        public void AfterInit()
        {
            SwapWeapon(0);
        }

        private void Update()
        {
            if (_playerInput == null || _currentWeapon == null) return;
            if (_currentWeapon.IsAutoFire && _playerInput.IsFireHeld)
                _currentWeapon.Fire();
        }

        private void OnFirePressed()
        {
            if (_currentWeapon == null || _currentWeapon.IsAutoFire) return;
            _currentWeapon.Fire();
        }

        private void SwapWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;

            _currentIndex = index;
            _currentWeapon = weapons[_currentIndex];

            switch (index)
            {
                case 0:
                    _playerVisualController.ChangeVisual(PlayerVisualState.PISTOL);
                    currentGunUI.SetPistolImage();
                    _currentWeapon.Equip();
                    break;
                case 1:
                    _playerVisualController.ChangeVisual(PlayerVisualState.SHOTGUN);
                    currentGunUI.SetShotGunImage();
                    _currentWeapon.Equip();
                    break;
                case 2:
                    _playerVisualController.ChangeVisual(PlayerVisualState.MACHINEGUN);
                    currentGunUI.SetMachineGunImage();
                    _currentWeapon.Equip();
                    break;
                default:
                    _playerVisualController.ChangeVisual(PlayerVisualState.ALL);
                    _currentWeapon.Equip();
                    break;
            }
        }

        private void OnScroll(float scroll)
        {
            if (scroll > 0) SwapNext();
            else if (scroll < 0) SwapPrev();
        }

        private void OnWeapon1() => SwapWeapon(0);
        private void OnWeapon2() => SwapWeapon(1);
        private void OnWeapon3() => SwapWeapon(2);

        public void Fire() => _currentWeapon.Fire();

        private void SwapNext()
        {
            if (_currentIndex < weapons.Length - 1)
                SwapWeapon(_currentIndex + 1);
        }

        private void SwapPrev()
        {
            if (_currentIndex > 0)
                SwapWeapon(_currentIndex - 1);
        }

        private void OnDestroy()
        {
            if (_playerInput == null) return;
            _playerInput.OnScrollWeaponInput -= OnScroll;
            _playerInput.OnWeapon1Pressed -= OnWeapon1;
            _playerInput.OnWeapon2Pressed -= OnWeapon2;
            _playerInput.OnWeapon3Pressed -= OnWeapon3;
            _playerInput.OnFireKeyPressed -= OnFirePressed;
        }
    }
}
