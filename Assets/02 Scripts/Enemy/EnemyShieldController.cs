using System.Collections;
using _02_Scripts.Agent;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class EnemyShieldController : MonoBehaviour, IModule, IAfterInitModule
    {
        [Header("발동 조건")]
        [Tooltip("HP가 이 비율 이하로 떨어지면 보호막 발동 (0.2 = 20%)")]
        [SerializeField] private float triggerHpRatio = 0.2f;
        [SerializeField] private float shieldDuration = 3f;

        [Header("재발동")]
        [SerializeField] private bool canRetrigger = false;
        [SerializeField] private float retriggerCooldown = 10f;

        private AgentHealth _health;
        private EnemyVfxController _vfx;

        private bool _isShieldActive;
        private bool _hasTriggeredOnce;
        private float _lastTriggerTime = float.MinValue;

        public void Initialize(ModuleOwner owner)
        {
            _health = owner.GetModule<AgentHealth>();
            _vfx = owner.GetModule<EnemyVfxController>();
        }

        public void AfterInit()
        {
            if (_health != null)
                _health.CurrentHp.OnValueChanged += OnHpChanged;
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.CurrentHp.OnValueChanged -= OnHpChanged;
        }

        private void OnHpChanged(int before, int current)
        {
            if (_isShieldActive) return;
            if (current <= 0) return; // 이미 죽는 거면 보호막 의미 없음
            if (_health.MaxHp <= 0) return;

            float ratio = (float)current / _health.MaxHp;
            if (ratio > triggerHpRatio) return;
            if (!CanTrigger()) return;

            StartCoroutine(ShieldRoutine());
        }

        private bool CanTrigger()
        {
            if (!_hasTriggeredOnce) return true;
            if (!canRetrigger) return false;
            return Time.time >= _lastTriggerTime + retriggerCooldown;
        }

        private IEnumerator ShieldRoutine()
        {
            _isShieldActive = true;
            _hasTriggeredOnce = true;
            _lastTriggerTime = Time.time;

            _health.IsInvincible = true;
            _vfx?.Play(EnemyVfxType.Shield);

            yield return new WaitForSeconds(shieldDuration);

            _health.IsInvincible = false;
            _vfx?.Stop(EnemyVfxType.Shield);

            _isShieldActive = false;
        }
    }
}