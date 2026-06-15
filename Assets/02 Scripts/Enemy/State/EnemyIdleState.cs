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
    
            if (_navEnemyRenderer.NavMeshAgent.isActiveAndEnabled)
            {
                _navEnemyRenderer.NavMeshAgent.ResetPath();
                _navEnemyRenderer.NavMeshAgent.velocity = Vector3.zero;
            }
    
            _renderer.PlayClip(_stateClipHash, 0, crossFadeDuration, layerIndex);
        }

        public override void Update()
        {
            base.Update();
            Debug.Log("IDLE STATE");
            
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
                if(enemy.CurrentTarget == null)
                    enemy.ChangeState(EnemyStateEnum.CHASE);
                    
                else if(Vector3.Distance(enemy.CurrentTarget.position,enemy.transform.position) 
                        > _navEnemyRenderer.NavMeshAgent.stoppingDistance)
                    enemy.ChangeState(EnemyStateEnum.CHASE);
            }

        }
    }
}