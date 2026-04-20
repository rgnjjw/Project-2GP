using System;
using _02_Scripts.Agent.Interface;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public class AgentHealth : MonoBehaviour,IModule
    {
        public IReadOnlyNotifyValue<int> CurrentHp => GetNotifyHp();
        public int MaxHp => _maxHp;
        public event Action OnDead;
        public bool IsDead => _isDead;

        [SerializeField] private int debugCurrentHp;
        [SerializeField] private bool debugIsDead;

        private Agent _owner;
        private NotifyValue<int> _currentHp;
        private int _maxHp;
        private bool _isDead;
        private bool _isInitialized;
        private NotifyValue<int> GetNotifyHp()
        {
            if (_currentHp == null)
            {
                _currentHp = new NotifyValue<int>();
            }
            return _currentHp;
        }

        private void OnEnable()
        {
            GetNotifyHp().OnValueChanged += OnValueChanged;
        }

        public void Init(int maxHp)
        {
            if (maxHp <= 0)
                return;

            _isDead = false;
            debugIsDead = false;
            _isInitialized = true;
            
            _maxHp = maxHp;
            
            GetNotifyHp().Value = maxHp;
            debugCurrentHp = maxHp;
        }

        private void OnValueChanged(int beforeHp, int currentHp)
        {
            if (!_isInitialized || _isDead)
                return;
            
            debugCurrentHp = currentHp;
                
            if (currentHp <= 0)
            {
                _isDead = true;
                debugCurrentHp = 0;
                debugIsDead = true;
                OnDead?.Invoke();
            }
        }

        public void ApplyDamage(int damage)
        {
            if (!_isInitialized || _isDead || damage <= 0)
                return;
            
            var hp = GetNotifyHp();
            hp.Value = Mathf.Max(0, hp.Value - damage);
        }

        public void ApplyHeal(int heal)
        {
            if (!_isInitialized || _isDead || heal <= 0)
                return;
            
            var hp = GetNotifyHp();
            hp.Value = Mathf.Min(_maxHp, hp.Value + heal);
        }

        private void OnDisable()
        {
            if (_currentHp != null)
                _currentHp.OnValueChanged -= OnValueChanged;
        }

        public void Initialize(ModuleOwner moduleOwner)
        {
            _owner = moduleOwner as Agent;
            
            if (_owner == null)
                return;
            
            _isDead = false;
            debugIsDead = false;
            _isInitialized = true;
            
            int initHealth = _owner.GetModule<IAgentData>().Health.Value;

            _maxHp = initHealth;
            
            GetNotifyHp().Value = initHealth;
            debugCurrentHp = initHealth;
        }
    }
}