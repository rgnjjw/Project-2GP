using _02_Scripts.Enemy.Skill;
using UnityEngine;

namespace _02_Scripts.Enemy.State
{
    public class EnemyAttackState : AbstractEnemyState
    {
        private readonly EnemySkillController _enemySkillController;
        private readonly NavEnemyRenderer _navEnemyRenderer;
        private readonly EnemyAnimationEvent _enemyAnimationEvent;
        private SkillSO _currentSkill;

        private bool _animEnded;
        private bool _skillEnded;

        public EnemyAttackState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            if(enemy == null) return;
            _navEnemyRenderer = enemy.GetModule<NavEnemyRenderer>();
            _enemySkillController = enemy.GetModule<EnemySkillController>();
            _enemyAnimationEvent = enemy.GetModule<EnemyAnimationEvent>();
        }

        public override void Enter(float crossFadeDuration, int layerIndex = 0)
        {
            base.Enter(crossFadeDuration, layerIndex);

            if (_navEnemyRenderer.NavMeshAgent.isActiveAndEnabled && _navEnemyRenderer.NavMeshAgent.isOnNavMesh)
            {
                _navEnemyRenderer.NavMeshAgent.ResetPath();
                _navEnemyRenderer.NavMeshAgent.velocity = Vector3.zero;
            }
            
            _navEnemyRenderer.UseForcedRotation = false;

            _currentSkill = _enemySkillController.GetAvailableSkill();

            if (_currentSkill == null)
            {
                enemy.ChangeState(EnemyStateEnum.IDLE);
                return;
            }

            _animEnded = false;
            _skillEnded = false;

            _enemyAnimationEvent.OnAttackEnd += HandleAttackEnd;
            _enemyAnimationEvent.OnPrepare += HandlePrepare;
            _enemyAnimationEvent.OnAttack += HandleAttack;
            _currentSkill.OnExecutionComplete += HandleSkillComplete;

            _renderer.PlayClip(_currentSkill.AnimParam.ParamHash, 0, crossFadeDuration, layerIndex);
            _currentSkill.ExecuteSkill(enemy);
        }

        private void HandlePrepare()
        {
            _navEnemyRenderer.UseForcedRotation = true;
        }

        private void HandleAttack()
        {
            _navEnemyRenderer.UseForcedRotation = false;
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
        public override void Exit()
        {
            base.Exit();
            _navEnemyRenderer.UseForcedRotation = false;
            _enemyAnimationEvent.OnAttackEnd -= HandleAttackEnd;
            _enemyAnimationEvent.OnPrepare -= HandlePrepare;
            _enemyAnimationEvent.OnAttack -= HandleAttack;
            if (_currentSkill != null)
                _currentSkill.OnExecutionComplete -= HandleSkillComplete;

            enemy.GetModule<EnemyLaserAimer>()?.StopAim();
        }
    }
}