using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Core.AnimationSystem;
using _02_Scripts.Enemy.Skill;
using _02_Scripts.Enemy.State;
using _02_Scripts.UI;
using _02_Scripts.UI.StyleBar;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class Enemy : Agent.Agent
    {
        // 살아있는(HP>0) 모든 적의 전역 레지스트리.
        // 웨이브 클리어 판정이 '웨이브로 스폰한 적'뿐 아니라 '소환된 적'까지 모두 포함하도록 한다.
        private static readonly HashSet<Enemy> AliveEnemies = new();
        public static int AliveCount => AliveEnemies.Count;

        [SerializeField] private AnimParamSO hitAnimParam;
        [SerializeField] private int hitLayerIndex = 1;

        public Transform CurrentTarget { get; set; }

        private EnemySkillController _enemySkillController;
        private NavEnemyRenderer _navEnemyRenderer;
        private AgentHealth _agentHealth;
        private CapsuleCollider _capsuleCollider;
        private HitFlash _hitFlash;

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
            if (_isDead) return;
            _isDead = true;

            // HP가 0이 된 즉시 '살아있는 적'에서 제외(사망 연출 중인 적은 위협으로 치지 않음).
            AliveEnemies.Remove(this);

            LevelManager.Instance.AddExp(10);

            if (_capsuleCollider != null)
                _capsuleCollider.enabled = false;

            ChangeState(EnemyStateEnum.DEAD);
            StyleManager.Instance.AddStyleScore(StyleAction.Kill);
        }

        private void OnDestroy()
        {
            // 사망 처리 없이 파괴되는 경우(맵 정리 등)에도 레지스트리에서 확실히 제외.
            AliveEnemies.Remove(this);

            if (_agentHealth != null)
                _agentHealth.CurrentHp.OnValueChanged -= OnDamaged;
        }

        public void ChangeState(EnemyStateEnum nextState)
            => stateMachine.ChangeState((int)nextState);
    }
}