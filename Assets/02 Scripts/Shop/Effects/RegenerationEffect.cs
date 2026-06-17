using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Shop.Effects
{
    [ShopChip("Regeneration")]
    public class RegenerationEffect : IShopPassiveEffect, ITickableEffect
    {
        private int _healPerSecond;
        private float _timer;
        private AgentHealth _playerHealth;

        public void OnEquip(Player.Player player, ShopChipDataSO chipData)
        {
            _playerHealth = player.GetModule<AgentHealth>();
            var data = (RegenerationShopChipDataSO)chipData;
            _healPerSecond = data.GetHealPerSecond(1);
            _timer = 0f;
        }

        public void OnUnequip(Player.Player player)
        {
            _playerHealth = null;
        }

        public void OnLevelUp(int newLevel, ShopChipDataSO chipData)
        {
            var data = (RegenerationShopChipDataSO)chipData;
            _healPerSecond = data.GetHealPerSecond(newLevel);
        }

        public void Tick(float deltaTime)
        {
            if (_playerHealth == null) return;
            _timer += deltaTime;
            if (_timer >= 1f)
            {
                _timer -= 1f;
                _playerHealth.ApplyHeal(_healPerSecond);
            }
        }
    }
}
