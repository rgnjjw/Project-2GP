using _02_Scripts.Enemy.Skill;
using UnityEngine;

namespace _02_Scripts.Enemy.State
{
    public class EnemyIdleState : AbstractEnemyState
    {
        private readonly EnemySkillController _skillController;
        private readonly EnemyDataContainer _enemyDataContainer;
        private readonly NavEnemyRenderer _navEnemyRenderer;
        private float _checkTimer;//연산비용을 줄이기 위해 사용함

        // 정지거리 경계에서 idle↔chase가 깜빡이지 않도록, 추격 재시작은 이만큼 더 멀어야 한다.
        private const float ChaseHysteresis = 0.6f;

        public EnemyIdleState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            if (enemy == null) return;
            _skillController = enemy.GetModule<EnemySkillController>();
            _enemyDataContainer = enemy.GetModule<EnemyDataContainer>();
            _navEnemyRenderer = enemy.GetModule<NavEnemyRenderer>();
        }

        public override void Enter(float crossFadeDuration, int layerIndex = 0)
        {
            base.Enter(crossFadeDuration, layerIndex);
            _checkTimer = 0.2f;

            if (_navEnemyRenderer.NavMeshAgent.isActiveAndEnabled && _navEnemyRenderer.NavMeshAgent.isOnNavMesh)
            {
                _navEnemyRenderer.NavMeshAgent.ResetPath();
                _navEnemyRenderer.NavMeshAgent.velocity = Vector3.zero;
            }

            _renderer.PlayClip(_stateClipHash, 0, crossFadeDuration, layerIndex);
        }

        public override void Update()
        {
            base.Update();

            // 대기 중에도 타깃을 바라보게 한다(벽 보고 멍때리지 않도록).
            _navEnemyRenderer.UseForcedRotation = enemy.CurrentTarget != null;

            _checkTimer += Time.deltaTime;
            if (_checkTimer < 0.2f) return;
            _checkTimer = 0f;

            if (_skillController.GetAvailableSkill() != null)
            {
                enemy.ChangeState(EnemyStateEnum.ATTACK);
                return;
            }

            if (_enemyDataContainer.ChaseRange.HasAnyInRange(enemy.transform))
            {
                if (enemy.CurrentTarget == null)
                    enemy.CurrentTarget = _enemyDataContainer.ChaseRange.GetClosest(enemy.transform);

                if (enemy.CurrentTarget == null) return;

                // 히스테리시스: 정지거리 + 버퍼보다 멀 때만 추격 재시작 → 경계 깜빡임 방지
                float dist = Vector3.Distance(enemy.CurrentTarget.position, enemy.transform.position);
                if (dist > _navEnemyRenderer.NavMeshAgent.stoppingDistance + ChaseHysteresis)
                    enemy.ChangeState(EnemyStateEnum.CHASE);
            }
            else
            {
                enemy.CurrentTarget = null; // 추격 범위 밖이면 타깃 해제
            }
        }

        public override void Exit()
        {
            base.Exit();
            _navEnemyRenderer.UseForcedRotation = false;
        }
    }
}
