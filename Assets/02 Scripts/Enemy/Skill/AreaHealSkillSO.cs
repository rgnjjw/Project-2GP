using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "AreaHealSkillSO", menuName = "Skill/AreaHealSkillSO", order = 0)]
    public class AreaHealSkillSO : HealSkillSO
    {
        [field: SerializeField] public int HealAmount { get; private set; } = 20;

        private EnemyAnimationEvent _animEvent;
        private EnemyVfxController _vfx;
        private Enemy _enemy;

        public override void ExecuteSkill(Enemy enemy)
        {
            _enemy = enemy;
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();
            _vfx = enemy.GetModule<EnemyVfxController>();

            _animEvent.OnAttack += HandleAttack;
            _animEvent.OnAttackEnd += HandleAttackEnd;
        }

        private void HandleAttack()
        {
            _animEvent.OnAttack -= HandleAttack;
            HealAllInRange();
            NotifyComplete();
        }

        // 광역힐 모션이 끝나면 이펙트를 끈다.
        private void HandleAttackEnd()
        {
            _animEvent.OnAttackEnd -= HandleAttackEnd;
            _vfx?.Stop(EnemyVfxType.AreaHealEffect);
        }

        private void HealAllInRange()
        {
            if (DamageAreaDetection != null)
            {
                foreach (var t in DamageAreaDetection.GetAllInRange(_enemy.transform))
                {
                    if (!IsValidTarget(_enemy, t, out var ally)) continue;

                    ally.GetModule<AgentHealth>()?.ApplyHeal(HealAmount);
                }
            }

            _vfx?.Play(EnemyVfxType.AreaHealEffect);
        }
    }
}