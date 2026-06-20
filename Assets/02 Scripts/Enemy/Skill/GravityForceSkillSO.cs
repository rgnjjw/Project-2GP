using System.Collections;
using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "GravityForceSkillSO", menuName = "Skill/GravityForceSkillSO", order = 0)]
    public class GravityForceSkillSO : SkillSO
    {
        [Header("끌어당기기 / 밀어내기")]
        [SerializeField] private float forceDuration = 1.5f;

        [Tooltip("양수면 끌어당김, 음수면 밀어냄")]
        [SerializeField] private float forceSpeed = 8f;

        [Tooltip("끌어당기기일 때만 적용. 이 거리보다 가까워지면 즉시 종료하고 폭발")]
        [SerializeField] private float minDistance = 1.5f;

        [Header("종료 시 폭발")]
        [SerializeField] private int explosionDamage = 25;

        public override void ExecuteSkill(Enemy enemy)
        {
            if (enemy == null)
                return;

            Transform target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            EnemyAnimationEvent animEvent = enemy.GetModule<EnemyAnimationEvent>();
            EnemyVfxController vfx = enemy.GetModule<EnemyVfxController>();

            if (animEvent == null)
            {
                enemy.StartCoroutine(ForceRoutine(enemy, target, vfx));
                return;
            }

            void HandleAttack()
            {
                animEvent.OnAttack -= HandleAttack;

                if (target == null)
                {
                    NotifyComplete();
                    return;
                }

                enemy.StartCoroutine(ForceRoutine(enemy, target, vfx));
            }

            animEvent.OnAttack += HandleAttack;
        }

        private IEnumerator ForceRoutine(Enemy enemy, Transform target, EnemyVfxController vfx)
        {
            if (enemy == null || target == null)
            {
                NotifyComplete();
                yield break;
            }

            Player.Player player = target.GetComponent<Player.Player>();
            AgentMover mover = player != null ? player.GetModule<AgentMover>() : null;

            if (player == null || mover == null)
            {
                NotifyComplete();
                yield break;
            }

            bool isPulling = forceSpeed > 0f;
            float elapsed = 0f;

            vfx?.Play(EnemyVfxType.GravityZone);
            mover.BeginSlide(Vector3.zero);

            while (elapsed < forceDuration)
            {
                if (enemy == null || player == null)
                    break;

                elapsed += Time.fixedDeltaTime;

                Vector3 toEnemy = enemy.transform.position - player.transform.position;
                toEnemy.y = 0f;

                float distance = toEnemy.magnitude;

                if (isPulling && distance <= minDistance)
                    break;

                Vector3 direction = distance > 0.0001f ? toEnemy.normalized : Vector3.zero;
                Vector3 forceVelocity = direction * forceSpeed;

                mover.UpdateSlideVelocity(forceVelocity);

                yield return new WaitForFixedUpdate();
            }

            mover.EndSlide();

            if (enemy != null)
                Explode(enemy, player, vfx);

            vfx?.Stop(EnemyVfxType.GravityZone);

            NotifyComplete();
        }

        private void Explode(Enemy enemy, Player.Player fallbackPlayer, EnemyVfxController vfx)
        {
            if (enemy == null)
                return;

            if (DamageAreaDetection != null)
            {
                foreach (var target in DamageAreaDetection.GetAllInRange(enemy.transform))
                {
                    if (!target.TryGetComponent<Player.Player>(out Player.Player player))
                        continue;

                    AgentHealth health = player.GetModule<AgentHealth>();

                    if (health != null)
                        health.ApplyDamage(explosionDamage);
                }
            }
            else if (fallbackPlayer != null)
            {
                AgentHealth health = fallbackPlayer.GetModule<AgentHealth>();

                if (health != null)
                    health.ApplyDamage(explosionDamage);
            }

            vfx?.Play(EnemyVfxType.Explosion);
        }
    }
}