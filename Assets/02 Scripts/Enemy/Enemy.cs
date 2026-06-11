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

        private float _lastHitAnimTime;

        protected override void Awake()
        {
            base.Awake();

            ChangeState(EnemyStateEnum.IDLE);

            _enemySkillController = GetModule<EnemySkillController>();
            _navEnemyRenderer = GetModule<NavEnemyRenderer>();
            _agentHealth = GetModule<AgentHealth>();
            _animationEvent = GetModule<EnemyAnimationEvent>();
            _capsuleCollider = GetComponent<CapsuleCollider>();

            _navEnemyRenderer.NavMeshAgent.stoppingDistance =
                _enemySkillController.GetMinSkillRange() - 0.2f;

            _agentHealth.CurrentHp.OnValueChanged += OnDamaged;
            _animationEvent.OnHitEnd += OnHitEndHandle;

            _navEnemyRenderer.Animator.SetLayerWeight(hitLayerIndex, 0f);
        }

        private void OnDamaged(int before, int current)
        {
            if (current <= 0)
                return;

            if (current >= before)
                return;

            if (Time.time < _lastHitAnimTime + 0.15f)
                return;

            _lastHitAnimTime = Time.time;

            Animator animator = _navEnemyRenderer.Animator;

            animator.SetLayerWeight(hitLayerIndex, 1f);
            animator.ResetTrigger(hitAnimParam.ParamHash);
            animator.SetTrigger(hitAnimParam.ParamHash);
        }

        public void OnHitEndHandle()
        {
            if (_navEnemyRenderer != null)
            {
                _navEnemyRenderer.Animator.SetLayerWeight(hitLayerIndex, 0f);
            }
        }

        protected override void OnDead()
        {
            StageManager.Instance.EnemyCount--;
            LevelManager.Instance.AddExp(10);

            if (_capsuleCollider != null)
                _capsuleCollider.enabled = false;

            ChangeState(EnemyStateEnum.DEAD);
        }

        private void OnDestroy()
        {
            if (_agentHealth != null)
                _agentHealth.CurrentHp.OnValueChanged -= OnDamaged;
            if (_animationEvent != null)
                _animationEvent.OnHitEnd -= OnHitEndHandle;
        }

        public void ChangeState(EnemyStateEnum nextState)
            => stateMachine.ChangeState((int)nextState);
    }
}