using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Enemy.Skill;
using _02_Scripts.Enemy.State;
using _02_Scripts.UI;
using _02_Scripts.UI.StyleBar;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class Enemy : Agent.Agent
    {
        private static readonly HashSet<Enemy> AliveEnemies = new();
        public static int AliveCount => AliveEnemies.Count;

        public Transform CurrentTarget { get; set; }

        private EnemySkillController _enemySkillController;
        private NavEnemyRenderer _navEnemyRenderer;
        private AgentHealth _agentHealth;
        private CapsuleCollider _capsuleCollider;
        private HitFlash _hitFlash;
        private EnemyVfxController _vfxController;

        private float _lastHitAnimTime;
        private bool _isDead;

        protected override void Awake()
        {
            base.Awake();

            AliveEnemies.Add(this);

            ChangeState(EnemyStateEnum.IDLE);

            _enemySkillController = GetModule<EnemySkillController>();
            _navEnemyRenderer = GetModule<NavEnemyRenderer>();
            _agentHealth = GetModule<AgentHealth>();
            _hitFlash = GetModule<HitFlash>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _vfxController = GetComponent<EnemyVfxController>();

            _navEnemyRenderer.NavMeshAgent.stoppingDistance = _enemySkillController.GetMinSkillRange() - 0.2f;

            _agentHealth.CurrentHp.OnValueChanged += OnDamaged;
            _agentHealth.CurrentHp.OnValueChanged += OnHealed;
        }

        private void OnHealed(int before, int current)
        {
            if (current >= before)
            {
               _vfxController?.Play(EnemyVfxType.HealedEffect);
            }
        }

        private void OnDamaged(int before, int current)
        {
            if (current <= 0) return;
            if (current >= before) return;
            if (Time.time < _lastHitAnimTime + 0.15f) return;

            _lastHitAnimTime = Time.time;
            _hitFlash?.Flash();
        }

        protected override void OnDead()
        {
            if (_isDead) return;
            _isDead = true;

            AliveEnemies.Remove(this);

            LevelManager.Instance.AddExp(10);

            if (_capsuleCollider != null)
                _capsuleCollider.enabled = false;

            ChangeState(EnemyStateEnum.DEAD);
            StyleManager.Instance.AddStyleScore(StyleAction.Kill);
        }

        private void OnDestroy()
        {
            AliveEnemies.Remove(this);

            if (_agentHealth != null)
                _agentHealth.CurrentHp.OnValueChanged -= OnDamaged;
        }

        public void ChangeState(EnemyStateEnum nextState)
            => stateMachine.ChangeState((int)nextState);
    }
}