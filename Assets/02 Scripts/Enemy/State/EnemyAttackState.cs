using _02_Scripts.Enemy.Skill;
using UnityEngine;

namespace _02_Scripts.Enemy.State
{
    public class EnemyAttackState : AbstractEnemyState
    {
        private readonly EnemySkillController _enemySkillController;
        private readonly NavEnemyRenderer _navEnemyRenderer;
        private readonly EnemyAnimationEvent _enemyAnimationEvent;
        private readonly EnemyDataContainer _enemyDataContainer;
        private SkillSO _currentSkill;
        
        public EnemyAttackState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            if(enemy == null) return;
            _navEnemyRenderer = enemy.GetModule<NavEnemyRenderer>();
            _enemySkillController = enemy.GetModule<EnemySkillController>();
            _enemyAnimationEvent = enemy.GetModule<EnemyAnimationEvent>();
            _enemyDataContainer = enemy.GetModule<EnemyDataContainer>();
        }

        public override void Enter(float crossFadeDuration, int layerIndex = 0)
        {
            base.Enter(crossFadeDuration, layerIndex);
            
            _navEnemyRenderer.NavMeshAgent.ResetPath();
            _navEnemyRenderer.NavMeshAgent.velocity = Vector3.zero;
            
            _currentSkill = _enemySkillController.GetAvailableSkill();
            
            if (_currentSkill == null)
            {
                enemy.ChangeState(EnemyStateEnum.IDLE);
                return;
            }

            _enemyAnimationEvent.OnAttackEnd += HandleAttackEnd;
            _enemyAnimationEvent.OnAttack += HandleAttack;
            
            _renderer.PlayClip(_currentSkill.AnimParam.ParamHash,0,crossFadeDuration, layerIndex);
        }

        private void HandleAttack()
        {
            _currentSkill.ExecuteSkill(enemy.transform);
        }


        private void HandleAttackEnd()
        {
            _enemySkillController.RecordSkillUsed(_currentSkill);

            if (!_enemyDataContainer.ChaseRange.HasAnyInRange(enemy.transform))
            {
                enemy.ChangeState(EnemyStateEnum.IDLE);
                return;
            }

            enemy.ChangeState(EnemyStateEnum.IDLE);
        }

        public override void Update()
        {
            base.Update();
            Debug.Log("Attack STATE");
        }

        public override void Exit()
        {
            base.Exit();
            _enemyAnimationEvent.OnAttackEnd -= HandleAttackEnd;
            _enemyAnimationEvent.OnAttack -= HandleAttack;
        }   
    }
}
