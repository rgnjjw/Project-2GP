using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Agent;
using UnityEngine;
using UnityEngine.AI;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "DashSkillSO", menuName = "Skill/DashSkillSO")]
    public class DashSkillSO : SkillSO
    {
        [field: SerializeField] public float DashSpeed { get; private set; } = 15f;
        [field: SerializeField] public float DashDistance { get; private set; } = 5f;
        [field: SerializeField] public int Damage { get; private set; } = 15;

        public override void ExecuteSkill(Enemy enemy)
        {
            // 애니메이션 재생 중 플레이어가 TargetFinder 범위를 벗어날 수 있으므로
            // 마지막으로 추적했던 CurrentTarget을 폴백으로 사용
            Transform target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            if (target == null)
            {
                NotifyComplete(enemy);
                return;
            }

            enemy.StartCoroutine(DashCoroutine(enemy, target.position));
        }

        private IEnumerator DashCoroutine(Enemy enemy, Vector3 targetPos)
        {
            NavEnemyRenderer navRenderer = enemy.GetModule<NavEnemyRenderer>();
            if (navRenderer == null) { NotifyComplete(enemy); yield break; }

            EnemyVfxController vfx = enemy.GetModule<EnemyVfxController>();

            NavMeshAgent agent = navRenderer.NavMeshAgent;
            if (agent == null || !agent.isOnNavMesh) { NotifyComplete(enemy); yield break; }

            Vector3 direction = (targetPos - enemy.transform.position).normalized;
            Vector3 dashDest = enemy.transform.position + direction * DashDistance;

            if (NavMesh.SamplePosition(dashDest, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                dashDest = hit.position;

            float originalSpeed = agent.speed;
            float originalAccel = agent.acceleration;
            bool originalUpdateRotation = agent.updateRotation;

            agent.speed = DashSpeed;
            agent.acceleration = 9999f;
            agent.updateRotation = false; // NavMeshAgent가 rotation을 덮어쓰지 못하게 차단
            agent.SetDestination(dashDest);

            // 대쉬 방향으로 회전 고정
            Quaternion dashRotation = Quaternion.LookRotation(direction);
            navRenderer.IsRotationLocked = true;
            enemy.transform.rotation = dashRotation;
            navRenderer.transform.rotation = dashRotation;

            vfx?.Play(EnemyVfxType.Dash);

            yield return null; // 경로 계산 대기 (이 전에 remainingDistance가 0이라 즉시 종료되는 버그 방지)

            float elapsed = 0f;
            float maxTime = DashDistance / DashSpeed + 0.5f;
            var hitTargets = new HashSet<Transform>();

            while (elapsed < maxTime)
            {
                elapsed += Time.deltaTime;

                // 매 프레임 회전 강제 유지 (NavMeshAgent 경로 계산 등으로 틀어지는 것 방지)
                enemy.transform.rotation = dashRotation;
                navRenderer.transform.rotation = dashRotation;

                if (DamageAreaDetection != null)
                {
                    foreach (var t in DamageAreaDetection.GetAllInRange(enemy.transform))
                    {
                        if (!hitTargets.Add(t)) continue;

                        if (t.TryGetComponent<Player.Player>(out var player))
                            player.GetModule<AgentHealth>().ApplyDamage(Damage);
                    }
                }

                if (!agent.pathPending && agent.isOnNavMesh && agent.remainingDistance < 0.25f)
                    break;

                yield return null;
            }

            agent.speed = originalSpeed;
            agent.acceleration = originalAccel;
            agent.updateRotation = originalUpdateRotation;
            
            if (agent.isActiveAndEnabled && agent.isOnNavMesh)
                agent.ResetPath();
            
            navRenderer.IsRotationLocked = false;

            vfx?.Stop(EnemyVfxType.Dash);

            NotifyComplete(enemy);
        }
    }
}
