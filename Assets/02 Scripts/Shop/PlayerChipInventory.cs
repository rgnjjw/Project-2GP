using System;
using System.Collections.Generic;
using _02_Scripts.Core.Utility;
using _02_Scripts.UI;
using UnityEngine;

namespace _02_Scripts.Shop
{
    public class PlayerChipInventory : MonoSingleton<PlayerChipInventory>
    {
        [SerializeField] private PassiveSlotTableSO passiveSlotTable;
        [SerializeField] private Player.Player player;

        private readonly HashSet<string> _ownedWeapons = new();
        private readonly Dictionary<string, int> _ownedPassives = new();
        private string[] _equippedPassiveSlots = Array.Empty<string>();
        private readonly Dictionary<int, IShopPassiveEffect> _activeEffects = new();
        private readonly Dictionary<string, ShopChipDataSO> _chipDataCache = new();

        public event Action<ShopChipDataSO> OnChipPurchased;
        public event Action<ShopChipDataSO, int> OnChipUpgraded;
        public event Action<ShopChipDataSO> OnChipSold;
        public event Action<ShopChipDataSO, int> OnPassiveEquipped;
        public event Action<ShopChipDataSO, int> OnPassiveUnequipped;
        public event Action OnInventoryChanged;

        protected override void Awake()
        {
            base.Awake();
            LevelManager.Instance.OnLevelUp += HandleLevelUp;
            RefreshSlotCount();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (LevelManager.Instance != null)
                LevelManager.Instance.OnLevelUp -= HandleLevelUp;
        }

        private void Update()
        {
            foreach (var effect in _activeEffects.Values)
            {
                if (effect is ITickableEffect tickable)
                    tickable.Tick(Time.deltaTime);
            }
        }

        private void HandleLevelUp(int newLevel) => RefreshSlotCount();

        private void RefreshSlotCount()
        {
            int newCount = passiveSlotTable.GetSlotCount(LevelManager.Instance.CurrentLevel);
            if (newCount == _equippedPassiveSlots.Length) return;

            var old = _equippedPassiveSlots;
            _equippedPassiveSlots = new string[newCount];

            for (int i = 0; i < Mathf.Min(old.Length, newCount); i++)
                _equippedPassiveSlots[i] = old[i];

            for (int i = newCount; i < old.Length; i++)
            {
                if (_activeEffects.TryGetValue(i, out var effect))
                {
                    effect.OnUnequip(player);
                    _activeEffects.Remove(i);
                }
            }

            OnInventoryChanged?.Invoke();
        }

        public bool IsOwned(string chipId) => _ownedWeapons.Contains(chipId) || _ownedPassives.ContainsKey(chipId);

        public int GetCurrentLevel(string chipId) => _ownedPassives.TryGetValue(chipId, out int lv) ? lv : 0;

        public int GetAvailableSlotCount() => _equippedPassiveSlots.Length;

        public int GetFreeSlotIndex()
        {
            for (int i = 0; i < _equippedPassiveSlots.Length; i++)
                if (_equippedPassiveSlots[i] == null) return i;
            return -1;
        }

        public IReadOnlyCollection<string> GetOwnedWeapons() => _ownedWeapons;

        public IReadOnlyDictionary<string, int> GetOwnedPassives() => _ownedPassives;

        public string[] GetEquippedSlots() => _equippedPassiveSlots;

        public void AddChip(ShopChipDataSO chip)
        {
            _chipDataCache[chip.ChipId] = chip;

            if (chip.Category == ChipCategory.Weapon)
                _ownedWeapons.Add(chip.ChipId);
            else
                _ownedPassives[chip.ChipId] = 1;

            OnChipPurchased?.Invoke(chip);
            OnInventoryChanged?.Invoke();
        }

        public void UpgradeChip(ShopChipDataSO chip)
        {
            if (!_ownedPassives.TryGetValue(chip.ChipId, out int currentLevel)) return;

            int newLevel = currentLevel + 1;
            _ownedPassives[chip.ChipId] = newLevel;

            for (int i = 0; i < _equippedPassiveSlots.Length; i++)
            {
                if (_equippedPassiveSlots[i] == chip.ChipId && _activeEffects.TryGetValue(i, out var effect))
                    effect.OnLevelUp(newLevel, chip);
            }

            OnChipUpgraded?.Invoke(chip, newLevel);
            OnInventoryChanged?.Invoke();
        }

        public void RemoveChip(ShopChipDataSO chip)
        {
            if (chip.Category == ChipCategory.Weapon)
            {
                _ownedWeapons.Remove(chip.ChipId);
            }
            else
            {
                for (int i = 0; i < _equippedPassiveSlots.Length; i++)
                {
                    if (_equippedPassiveSlots[i] == chip.ChipId)
                    {
                        UnequipPassive(i);
                        break;
                    }
                }
                _ownedPassives.Remove(chip.ChipId);
            }

            _chipDataCache.Remove(chip.ChipId);
            OnChipSold?.Invoke(chip);
            OnInventoryChanged?.Invoke();
        }

        public bool TryEquipPassive(ShopChipDataSO chip, int slotIndex)
        {
            if (chip.Category != ChipCategory.Passive) return false;
            if (!_ownedPassives.ContainsKey(chip.ChipId)) return false;
            if (slotIndex < 0 || slotIndex >= _equippedPassiveSlots.Length) return false;

            if (_equippedPassiveSlots[slotIndex] != null)
                UnequipPassive(slotIndex);

            for (int i = 0; i < _equippedPassiveSlots.Length; i++)
            {
                if (_equippedPassiveSlots[i] == chip.ChipId)
                {
                    UnequipPassive(i);
                    break;
                }
            }

            _equippedPassiveSlots[slotIndex] = chip.ChipId;

            var effect = ShopChipFactory.Create(chip.ChipId);
            if (effect != null)
            {
                effect.OnEquip(player, chip);
                _activeEffects[slotIndex] = effect;
            }

            OnPassiveEquipped?.Invoke(chip, slotIndex);
            return true;
        }

        public void UnequipPassive(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= _equippedPassiveSlots.Length) return;
            if (_equippedPassiveSlots[slotIndex] == null) return;

            string chipId = _equippedPassiveSlots[slotIndex];
            _equippedPassiveSlots[slotIndex] = null;

            if (_activeEffects.TryGetValue(slotIndex, out var effect))
            {
                effect.OnUnequip(player);
                _activeEffects.Remove(slotIndex);
            }

            if (_chipDataCache.TryGetValue(chipId, out var chipData))
                OnPassiveUnequipped?.Invoke(chipData, slotIndex);
        }

        public bool TryAutoEquipPassive(ShopChipDataSO chip)
        {
            int freeSlot = GetFreeSlotIndex();
            if (freeSlot < 0) return false;
            return TryEquipPassive(chip, freeSlot);
        }
    }
}
