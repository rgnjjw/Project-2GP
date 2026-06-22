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
            _navEnemyRenderer.UseInstantForcedRotation = false;

            _currentSkill = _enemySkillController.GetAvailableSkill();

            if (_currentSkill == null)
            {
                enemy.ChangeState(EnemyStateEnum.IDLE);
                return;
            }

            // 공격(특히 원거리)은 발사 순간까지 플레이어를 즉시 추적하도록 한다.
            // 힐 스킬은 아군을 바라봐야 하므로 제외.
            _navEnemyRenderer.UseInstantForcedRotation = !_currentSkill.IsHealSkill;

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
            // 예열 시작 시점에 즉시 한 번 정렬(이후엔 UseInstantForcedRotation이 매 프레임 추적).
            _navEnemyRenderer.SnapLookAtTarget();
        }

        private void HandleAttack()
        {
            // 발사 시점에도 계속 즉시 추적(애니메이션 끝날 때까지). 추적 종료는 Exit에서 처리.
            // 탄/빔은 이 프레임의 바라보는 방향으로 나가므로 조준은 정확하다.
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
            _navEnemyRenderer.UseInstantForcedRotation = false;
            _enemyAnimationEvent.OnAttackEnd -= HandleAttackEnd;
            _enemyAnimationEvent.OnPrepare -= HandlePrepare;
            _enemyAnimationEvent.OnAttack -= HandleAttack;
            if (_currentSkill != null)
                _currentSkill.OnExecutionComplete -= HandleSkillComplete;

            enemy.GetModule<EnemyLaserAimer>()?.StopAim();
        }
    }
}