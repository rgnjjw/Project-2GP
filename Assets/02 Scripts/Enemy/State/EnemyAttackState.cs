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

            // 스냅 회전이 향할 대상을 확실히 갱신해 둔다(아이들에서 바로 진입한 경우 CurrentTarget이 비어있을 수 있음).
            if (_currentSkill.TargetFinder != null)
            {
                var t = _currentSkill.TargetFinder.GetClosest(enemy.transform);
                if (t != null) enemy.CurrentTarget = t;
            }

            _animEnded = false;
            _skillEnded = false;

            // 이전 공격이 OnAttack 전에 중단됐다면 스킬이 남긴 익명 핸들러가 그대로 붙어 있을 수 있다.
            // 새 공격을 등록하기 전에 비워, 한 번의 OnAttack에 데미지가 중복 적용되는 것을 막는다.
            _enemyAnimationEvent.ClearAttackEvents();

            _enemyAnimationEvent.OnAttackEnd += HandleAttackEnd;
            _enemyAnimationEvent.OnPrepare += HandlePrepare;
            _enemyAnimationEvent.OnAttack += HandleAttack;
            _currentSkill.OnExecutionComplete += HandleSkillComplete;

            _renderer.PlayClip(_currentSkill.AnimParam.ParamHash, 0, crossFadeDuration, layerIndex);
            _currentSkill.ExecuteSkill(enemy);
        }

        private void HandlePrepare()
        {
            // 조준 시작 시 단 한 번만 플레이어 방향으로 회전하고 고정(이후 추적 회전 없음).
            _navEnemyRenderer.SnapLookAtTarget();
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

        private void HandleSkillComplete(Enemy completedEnemy)
        {
            // 같은 스킬 에셋을 공유하는 다른 적의 완료 신호는 무시(교차 오발 방지).
            if (completedEnemy != enemy) return;
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