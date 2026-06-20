using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "SingleHealSkillSO", menuName = "Skill/SingleHealSkillSO", order = 0)]
    public class SingleHealSkillSO : HealSkillSO
    {
        [field: SerializeField] public int HealAmount { get; private set; } = 30;

        private EnemyAnimationEvent _animEvent;
        private Enemy _enemy;
        private Transform _healTarget;
        private NavEnemyRenderer _navRenderer;

        public override void ExecuteSkill(Enemy enemy)
        {
            _enemy = enemy;
            _healTarget = HealTargeting.GetLowestHp(enemy, TargetFinder);
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();
            _navRenderer = enemy.GetModule<NavEnemyRenderer>();

            // 힐 대상(아군) 쪽으로 즉시 시선을 스냅한다(요청: 바로 회전).
            // Prepare 시점에 한 번 더 보정(공격 상태가 플레이어 쪽으로 스냅하는 것보다 나중에 실행되어 덮어씀).
            FaceHealTarget();
            _animEvent.OnPrepare += HandlePrepare;
            _animEvent.OnAttack += HandleAttack;
        }

        private void HandlePrepare()
        {
            _animEvent.OnPrepare -= HandlePrepare;
            FaceHealTarget();
        }

        private void FaceHealTarget()
        {
            if (_healTarget != null)
                _navRenderer?.SnapLookAt(_healTarget.position);
        }

        private void HandleAttack()
        {
            _animEvent.OnAttack -= HandleAttack;
            _animEvent.OnPrepare -= HandlePrepare;

            if (_healTarget != null && _healTarget.TryGetComponent<Enemy>(out var ally))
                ally.GetModule<AgentHealth>()?.ApplyHeal(HealAmount);

            // _vfx?.Play(EnemyVfxType.None); 이건 힐받은놈이 이펙트로 교체함

            NotifyComplete();
        }
    }
}