using _02_Scripts.Enemy.Skill;
using UnityEngine;

namespace _02_Scripts.Enemy.State
{
    public class EnemyChaseState : AbstractEnemyState
    {
        private readonly NavEnemyRenderer _navEnemyRenderer;
        private readonly EnemySkillController _enemySkillController;
        private readonly EnemyDataContainer _enemyDataContainer;
        private float _checkTimer;

        public EnemyChaseState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            if (enemy == null) return;
            _navEnemyRenderer = enemy.GetModule<NavEnemyRenderer>();
            _enemyDataContainer = enemy.GetModule<EnemyDataContainer>();
            _enemySkillController = enemy.GetModule<EnemySkillController>();
        }

        public override void Enter(float crossFadeDuration, int layerIndex = 0)
        {
            base.Enter(crossFadeDuration, layerIndex);
            _checkTimer = 0.2f;
            _navEnemyRenderer.UseChaseRotation = true;

            enemy.CurrentTarget = Skill.HealTargeting.SelectChaseTarget(enemy, _enemyDataContainer.ChaseRange);
            if (enemy.CurrentTarget != null && _navEnemyRenderer.NavMeshAgent.isActiveAndEnabled)
                _navEnemyRenderer.NavMeshAgent.SetDestination(enemy.CurrentTarget.position);

            _renderer.PlayClip(_stateClipHash, 0, crossFadeDuration, layerIndex);

            float normalizedSpeed = _navEnemyRenderer.NavMeshAgent.velocity.magnitude / _navEnemyRenderer.NavMeshAgent.speed;
            _renderer.Animator.SetFloat(_navEnemyRenderer.SpeedAnimParam.ParamHash, normalizedSpeed, 0.1f, Time.deltaTime);
        }

        public override void Update()
        {
            base.Update();

            Vector3 velocity = _navEnemyRenderer.NavMeshAgent.velocity;
            float normalizedSpeed = velocity.magnitude / _navEnemyRenderer.NavMeshAgent.speed;
            _renderer.Animator.SetFloat(_navEnemyRenderer.SpeedAnimParam.ParamHash, normalizedSpeed, 0.1f, Time.deltaTime);

            _checkTimer += Time.deltaTime;
            if (_checkTimer < 0.2f) return;
            _checkTimer = 0f;

            if (_enemySkillController.GetAvailableSkill() != null)
            {
                enemy.ChangeState(EnemyStateEnum.ATTACK);
                return;
            }

            if (!_enemyDataContainer.ChaseRange.HasAnyInRange(enemy.transform)
                || Vector3.Distance(enemy.CurrentTarget.position, enemy.transform.position) <= _navEnemyRenderer.NavMeshAgent.stoppingDistance)
            {
                enemy.ChangeState(EnemyStateEnum.IDLE);
                return;
            }

            var target = Skill.HealTargeting.SelectChaseTarget(enemy, _enemyDataContainer.ChaseRange);
            if (target != null)
            {
                enemy.CurrentTarget = target;
                if (_navEnemyRenderer.NavMeshAgent.isActiveAndEnabled)
                    _navEnemyRenderer.NavMeshAgent.SetDestination(target.position);
            }
        }

        public override void Exit()
        {
            base.Exit();
            _navEnemyRenderer.UseChaseRotation = false;
        }
    }
}
