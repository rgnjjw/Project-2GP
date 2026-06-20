using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "SingleHealSkillSO", menuName = "Skill/SingleHealSkillSO", order = 0)]
    public class SingleHealSkillSO : HealSkillSO
    {
        [field: SerializeField] public int HealAmount { get; private set; } = 30;

        public override void ExecuteSkill(Enemy enemy)
        {
            if (enemy == null)
                return;

            Transform healTarget = HealTargeting.GetLowestHp(enemy, TargetFinder);
            EnemyAnimationEvent animEvent = enemy.GetModule<EnemyAnimationEvent>();
            NavEnemyRenderer navRenderer = enemy.GetModule<NavEnemyRenderer>();
            EnemyVfxController vfx = enemy.GetModule<EnemyVfxController>();

            FaceHealTarget(healTarget, navRenderer);

            if (animEvent == null)
            {
                HealTarget(healTarget);
                vfx?.Play(EnemyVfxType.SingleHealEffect);
                NotifyComplete();
                return;
            }

            void HandlePrepare()
            {
                animEvent.OnPrepare -= HandlePrepare;
                FaceHealTarget(healTarget, navRenderer);
            }

            void HandleAttack()
            {
                animEvent.OnPrepare -= HandlePrepare;
                animEvent.OnAttack -= HandleAttack;

                HealTarget(healTarget);

                vfx?.Play(EnemyVfxType.SingleHealEffect);

                NotifyComplete();
            }

            animEvent.OnPrepare += HandlePrepare;
            animEvent.OnAttack += HandleAttack;
        }

        private void FaceHealTarget(Transform healTarget, NavEnemyRenderer navRenderer)
        {
            if (healTarget == null || navRenderer == null)
                return;

            navRenderer.SnapLookAt(healTarget.position);
        }

        private void HealTarget(Transform healTarget)
        {
            if (healTarget == null)
                return;

            if (!healTarget.TryGetComponent<Enemy>(out Enemy ally))
                return;

            AgentHealth health = ally.GetModule<AgentHealth>();

            if (health != null)
                health.ApplyHeal(HealAmount);
        }
    }
}