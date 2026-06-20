using System.Collections;
using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "SelfDestructSkillSO", menuName = "Skill/SelfDestructSkillSO", order = 0)]
    public class SelfDestructSkillSO : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; } = 40;

        [Header("자폭 타이밍")]
        [Tooltip("폭발(OnAttack) 이후, 실제로 죽기까지 추가 지연 (이펙트 보여줄 시간)")]
        [SerializeField] private float deathDelay = 0.1f;

        public override void ExecuteSkill(Enemy enemy)
        {
            if (enemy == null)
                return;

            EnemyAnimationEvent animEvent = enemy.GetModule<EnemyAnimationEvent>();
            EnemyVfxController vfx = enemy.GetModule<EnemyVfxController>();

            if (animEvent == null)
            {
                Explode(enemy, vfx);
                return;
            }

            void HandleAttack()
            {
                animEvent.OnAttack -= HandleAttack;
                Explode(enemy, vfx);
            }

            animEvent.OnAttack -= HandleAttack;
            animEvent.OnAttack += HandleAttack;
        }

        private void Explode(Enemy enemy, EnemyVfxController vfx)
        {
            if (enemy == null)
                return;

            if (DamageAreaDetection != null)
            {
                foreach (var target in DamageAreaDetection.GetAllInRange(enemy.transform))
                {
                    if (target.TryGetComponent<Player.Player>(out var player))
                    {
                        AgentHealth playerHealth = player.GetModule<AgentHealth>();

                        if (playerHealth != null)
                            playerHealth.ApplyDamage(Damage);
                    }
                }
            }

            vfx?.Play(EnemyVfxType.Explosion);

            enemy.StartCoroutine(KillSelfAfterDelay(enemy));
        }

        private IEnumerator KillSelfAfterDelay(Enemy enemy)
        {
            NotifyComplete();

            if (deathDelay > 0f)
                yield return new WaitForSeconds(deathDelay);

            if (enemy == null)
                yield break;

            AgentHealth health = enemy.GetModule<AgentHealth>();

            if (health != null)
                health.ApplyDamage(int.MaxValue);
        }
    }
}