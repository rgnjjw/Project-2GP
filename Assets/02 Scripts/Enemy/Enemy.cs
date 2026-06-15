using _02_Scripts.Agent;
using _02_Scripts.Core.AnimationSystem;
using _02_Scripts.Enemy.Skill;
using _02_Scripts.Enemy.State;
using _02_Scripts.Manager;
using _02_Scripts.UI;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class Enemy : Agent.Agent
    {
        [SerializeField] private AnimParamSO hitAnimParam;
        [SerializeField] private int hitLayerIndex = 1;

        public Transform CurrentTarget { get; set; }

        private EnemyAnimationEvent _animationEvent;
        private EnemySkillController _enemySkillController;
        private NavEnemyRenderer _navEnemyRenderer;
        private AgentHealth _agentHealth;
        private CapsuleCollider _capsuleCollider;
        private HitFlash _hitFlash;

        private float _lastHitAnimTime;

        protected override void Awake()
        {
            base.Awake();

            ChangeState(EnemyStateEnum.IDLE);

            _enemySkillController = GetModule<EnemySkillController>();
            _navEnemyRenderer = GetModule<NavEnemyRenderer>();
            _agentHealth = GetModule<AgentHealth>();
            _animationEvent = GetModule<EnemyAnimationEvent>();
            _hitFlash = GetModule<HitFlash>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            _navEnemyRenderer.NavMeshAgent.stoppingDistance =
                _enemySkillController.GetMinSkillRange() - 0.2f;

            _agentHealth.CurrentHp.OnValueChanged += OnDamaged;

            _navEnemyRenderer.Animator.SetLayerWeight(hitLayerIndex, 0f);
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
            LevelManager.Instance.AddExp(10);

            if (_capsuleCollider != null)
                _capsuleCollider.enabled = false;

            ChangeState(EnemyStateEnum.DEAD);
        }

        private void OnDestroy()
        {
            if (_agentHealth != null)
                _agentHealth.CurrentHp.OnValueChanged -= OnDamaged;
        }

        public void ChangeState(EnemyStateEnum nextState)
            => stateMachine.ChangeState((int)nextState);
    }
}