using _02_Scripts.Agent;
using _02_Scripts.Core.Utility;
using _02_Scripts.Gun;
using UnityEngine;

namespace _02_Scripts.Shop.Effects
{
    [ShopChip("LifeSteal")]
    public class LifeStealEffect : IShopPassiveEffect
    {
        private float _currentRate;
        private AgentHealth _playerHealth;

        public void OnEquip(Player.Player player, ShopChipDataSO chipData)
        {
            _playerHealth = player.GetModule<AgentHealth>();
            var data = (LifeStealShopChipDataSO)chipData;
            _currentRate = data.GetStealRate(1);
            EventBus.Subscribe<PlayerDamageDealtEvent>(OnDamageDealt);
        }

        public void OnUnequip(Player.Player player)
        {
            EventBus.Unsubscribe<PlayerDamageDealtEvent>(OnDamageDealt);
            _playerHealth = null;
        }

        public void OnLevelUp(int newLevel, ShopChipDataSO chipData)
        {
            var data = (LifeStealShopChipDataSO)chipData;
            _currentRate = data.GetStealRate(newLevel);
        }

        private void OnDamageDealt(PlayerDamageDealtEvent e)
        {
            int heal = Mathf.Max(1, Mathf.FloorToInt(e.Damage * _currentRate));
            _playerHealth?.ApplyHeal(heal);
        }
    }
}
