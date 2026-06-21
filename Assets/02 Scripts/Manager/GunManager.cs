using System;
using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Player;
using _02_Scripts.Shop;
using _02_Scripts.UI;
using UnityEngine;

namespace _02_Scripts.Manager
{
    public class GunManager : MonoBehaviour, IModule, IAfterInitModule
    {
        public event Action<Gun.Gun> OnWeaponChanged;

        [SerializeField] private Gun.Gun[] weapons;
        [SerializeField] private CurrentGunUI currentGunUI;
        private Gun.Gun _currentWeapon;
        private int _currentIndex;
        private PlayerInputSO _playerInput;
        private PlayerVisualController _playerVisualController;
        private readonly HashSet<int> _unlockedWeapons = new();

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
                _playerInput.OnSkillKeyPressed += OnSkillPressed;
                _playerInput.OnSkillKeyReleased += OnSkillReleased;
            }
        }

        public void AfterInit()
        {
            _unlockedWeapons.Add(0);
            _unlockedWeapons.Add(1);
            _unlockedWeapons.Add(2);
            if (ShopManager.Instance != null)
                ShopManager.Instance.OnWeaponUnlocked += UnlockWeapon;
            SwapWeapon(0);
        }

        public void UnlockWeapon(int index)
        {
            _unlockedWeapons.Add(index);
        }

        // 무기 스킬 레벨 설정 (상점 강화 / 레벨업 등에서 호출). 레벨이 오르면 대미지 증가·쿨타임 감소.
        public void SetWeaponSkillLevel(int weaponIndex, int level)
        {
            if (weaponIndex < 0 || weaponIndex >= weapons.Length) return;
            weapons[weaponIndex].SetSkillLevel(level);
        }

        private void Update()
        {
            if (_playerInput == null || _currentWeapon == null) return;
            if (Mathf.Approximately(Time.timeScale, 0f)) return;
            if (PauseManager.Instance != null && PauseManager.Instance.JustResumed) return;
            if (_currentWeapon.IsAutoFire && _playerInput.IsFireHeld)
                _currentWeapon.Fire();

            // 스킬 쿨타임은 장착 여부와 무관하게 흘러야 하므로 모든 무기의 스킬을 틱한다.
            // (다른 무기를 들고 있어도 머신건 스킬 쿨이 계속 줄어들도록)
            for (int i = 0; i < weapons.Length; i++)
                weapons[i].TickSkill(Time.deltaTime);
        }

        private void OnFirePressed()
        {
            if (_currentWeapon == null || _currentWeapon.IsAutoFire) return;
            if (Mathf.Approximately(Time.timeScale, 0f)) return;
            if (PauseManager.Instance != null && PauseManager.Instance.JustResumed) return;
            _currentWeapon.Fire();
        }

        private void OnSkillPressed()
        {
            if (_currentWeapon == null) return;
            if (Mathf.Approximately(Time.timeScale, 0f)) return;
            if (PauseManager.Instance != null && PauseManager.Instance.JustResumed) return;
            _currentWeapon.OnSkillPressed();
        }

        private void OnSkillReleased()
        {
            if (_currentWeapon == null) return;
            _currentWeapon.OnSkillReleased();
        }

        private void SwapWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;
            if (!_unlockedWeapons.Contains(index)) return;

            // 이전 무기 해제 처리(전기톱 루프 사운드 등 진행 중 스킬 정리).
            if (_currentWeapon != null)
                _currentWeapon.OnUnequip();

            _currentIndex = index;
            _currentWeapon = weapons[_currentIndex];
            OnWeaponChanged?.Invoke(_currentWeapon);

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
            for (int i = _currentIndex + 1; i < weapons.Length; i++)
            {
                if (_unlockedWeapons.Contains(i)) { SwapWeapon(i); return; }
            }
        }

        private void SwapPrev()
        {
            for (int i = _currentIndex - 1; i >= 0; i--)
            {
                if (_unlockedWeapons.Contains(i)) { SwapWeapon(i); return; }
            }
        }

        private void OnDestroy()
        {
            if (_playerInput != null)
            {
                _playerInput.OnScrollWeaponInput -= OnScroll;
                _playerInput.OnWeapon1Pressed -= OnWeapon1;
                _playerInput.OnWeapon2Pressed -= OnWeapon2;
                _playerInput.OnWeapon3Pressed -= OnWeapon3;
                _playerInput.OnFireKeyPressed -= OnFirePressed;
                _playerInput.OnSkillKeyPressed -= OnSkillPressed;
                _playerInput.OnSkillKeyReleased -= OnSkillReleased;
            }
            if (ShopManager.Instance != null)
                ShopManager.Instance.OnWeaponUnlocked -= UnlockWeapon;
        }
    }
}
