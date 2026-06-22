using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "MeleeAttackSkill", menuName = "Skill/MeleeAttackSkill", order = 0)]
    public class MeleeAttackSkill : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public EnemyVfxType  VfxType { get; private set; }

        public override void ExecuteSkill(Enemy enemy)
        {
            var animEvent = enemy.GetModule<EnemyAnimationEvent>();
            var vfx = enemy.GetModule<EnemyVfxController>();

            void HandleAttack()
            {
                animEvent.OnAttack -= HandleAttack;

                var closest = DamageAreaDetection.GetClosest(enemy.transform);
                if (closest != null && closest.TryGetComponent<Player.Player>(out var player))
                    player.GetModule<AgentHealth>().ApplyDamage(Damage);

                vfx?.Play(VfxType);

                NotifyComplete(enemy);
            }

            animEvent.OnAttack += HandleAttack;
        }
    }
}
