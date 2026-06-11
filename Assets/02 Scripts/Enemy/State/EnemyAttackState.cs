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

        private bool _animEnded;
        private bool _skillEnded;

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

            _animEnded = false;
            _skillEnded = false;

            _enemyAnimationEvent.OnAttackEnd += HandleAttackEnd;
            _enemyAnimationEvent.OnAttack += HandleAttack;
            _currentSkill.OnExecutionComplete += HandleSkillComplete;

            _renderer.PlayClip(_currentSkill.AnimParam.ParamHash, 0, crossFadeDuration, layerIndex);
        }

        private void HandleAttack()
        {
            _currentSkill.ExecuteSkill(enemy.transform);
        }

        private void HandleAttackEnd()
        {
            _animEnded = true;
            TryTransition();
        }

        private void HandleSkillComplete()
        {
            _skillEnded = true;
            TryTransition();
        }

        private void TryTransition()
        {
            if (!_animEnded || !_skillEnded) return;

            _enemySkillController.RecordSkillUsed(_currentSkill);
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
            if (_currentSkill != null)
                _currentSkill.OnExecutionComplete -= HandleSkillComplete;
        }
    }
}
