using _02_Scripts.Agent;

namespace _02_Scripts.Shop.Effects
{
    [ShopChip("Invincibility")]
    public class InvincibilityEffect : IShopPassiveEffect, ITickableEffect
    {
        private float _hpThreshold;
        private float _duration;
        private float _cooldown;

        private float _cooldownTimer;
        private float _invincibleTimer;
        private bool _isInvincible;

        private AgentHealth _playerHealth;

        public void OnEquip(Player.Player player, ShopChipDataSO chipData)
        {
            _playerHealth = player.GetModule<AgentHealth>();
            var data = (InvincibilityShopChipDataSO)chipData;
            _hpThreshold = data.HpThreshold;
            _duration = data.InvincibleDuration;
            _cooldown = data.GetCooldown(1);
            _cooldownTimer = 0f;
            _playerHealth.CurrentHp.OnValueChanged += OnHpChanged;
        }

        public void OnUnequip(Player.Player player)
        {
            _playerHealth.CurrentHp.OnValueChanged -= OnHpChanged;
            if (_isInvincible)
                _playerHealth.IsInvincible = false;
            _playerHealth = null;
        }

        public void OnLevelUp(int newLevel, ShopChipDataSO chipData)
        {
            var data = (InvincibilityShopChipDataSO)chipData;
            _cooldown = data.GetCooldown(newLevel);
        }

        private void OnHpChanged(int before, int current)
        {
            if (_isInvincible || _cooldownTimer > 0) return;
            float ratio = (float)current / _playerHealth.MaxHp;
            if (ratio <= _hpThreshold && current < before)
            {
                _isInvincible = true;
                _invincibleTimer = _duration;
                _playerHealth.IsInvincible = true;
            }
        }

        public void Tick(float deltaTime)
        {
            if (_cooldownTimer > 0)
                _cooldownTimer -= deltaTime;

            if (_isInvincible)
            {
                _invincibleTimer -= deltaTime;
                if (_invincibleTimer <= 0f)
                {
                    _isInvincible = false;
                    _playerHealth.IsInvincible = false;
                    _cooldownTimer = _cooldown;
                }
            }
        }
    }
}
