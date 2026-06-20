using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "AreaHealSkillSO", menuName = "Skill/AreaHealSkillSO", order = 0)]
    public class AreaHealSkillSO : HealSkillSO
    {
        [field: SerializeField] public int HealAmount { get; private set; } = 20;

        public override void ExecuteSkill(Enemy enemy)
        {
            if (enemy == null)
                return;

            EnemyAnimationEvent animEvent = enemy.GetModule<EnemyAnimationEvent>();
            EnemyVfxController vfx = enemy.GetModule<EnemyVfxController>();

            if (animEvent == null)
            {
                HealAllInRange(enemy, vfx);
                NotifyComplete();
                return;
            }

            void HandleAttack()
            {
                animEvent.OnAttack -= HandleAttack;

                HealAllInRange(enemy, vfx);
                NotifyComplete();
            }

            void HandleAttackEnd()
            {
                animEvent.OnAttackEnd -= HandleAttackEnd;
                vfx?.Stop(EnemyVfxType.AreaHealEffect);
            }

            animEvent.OnAttack += HandleAttack;
            animEvent.OnAttackEnd += HandleAttackEnd;
        }

        private void HealAllInRange(Enemy enemy, EnemyVfxController vfx)
        {
            if (enemy == null)
                return;

            if (DamageAreaDetection != null)
            {
                foreach (var target in DamageAreaDetection.GetAllInRange(enemy.transform))
                {
                    if (!IsValidTarget(enemy, target, out Enemy ally))
                        continue;

                    AgentHealth allyHealth = ally.GetModule<AgentHealth>();

                    if (allyHealth != null)
                        allyHealth.ApplyHeal(HealAmount);
                }
            }

            vfx?.Play(EnemyVfxType.AreaHealEffect);
        }
    }
}