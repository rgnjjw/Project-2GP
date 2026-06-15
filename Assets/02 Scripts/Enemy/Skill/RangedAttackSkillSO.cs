using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "RangedAttackSkillSO", menuName = "Skill/RangedAttackSkillSO", order = 0)]
    public class RangedAttackSkillSO : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }
        [field: SerializeField] public TrailRenderer TracerPrefab { get; private set; }
        [field: SerializeField] public LineRenderer LaserPrefab { get; private set; }

        public override void ExecuteSkill(Enemy enemy)
        {
            var target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            var muzzle = FindMuzzle(enemy);
            var animEvent = enemy.GetModule<EnemyAnimationEvent>();
            var laserAimer = enemy.GetModule<EnemyLaserAimer>();

            Debug.Log($"[RangedSkill] ExecuteSkill 시작 | target={target?.name ?? "null"} | muzzle={muzzle?.name} | animEvent={animEvent != null} | laserAimer={laserAimer != null}");

            void HandlePrepare()
            {
                Debug.Log("[RangedSkill] OnPrepare 발동 - 레이저 조준 시작");
                if (LaserPrefab != null && laserAimer != null)
                    laserAimer.StartAim(LaserPrefab, muzzle, target, TargetLayer);
                else
                    Debug.LogWarning($"[RangedSkill] OnPrepare - LaserPrefab={LaserPrefab != null} / laserAimer={laserAimer != null}");
            }

            void HandleAttack()
            {
                Debug.Log("[RangedSkill] OnAttack 발동 - 발사");
                laserAimer?.StopAim();

                animEvent.OnPrepare -= HandlePrepare;
                animEvent.OnAttack -= HandleAttack;

                if (target != null)
                {
                    Vector3 dir = (target.position - muzzle.position).normalized;
                    FireTracer(muzzle.position, dir);
                }
                else
                {
                    Debug.LogWarning("[RangedSkill] OnAttack - target이 null이라 발사 스킵");
                }

                NotifyComplete();
            }

            animEvent.OnPrepare += HandlePrepare;
            animEvent.OnAttack += HandleAttack;
        }

        private void FireTracer(Vector3 origin, Vector3 direction)
        {
            if (TracerPrefab == null)
            {
                Debug.LogWarning("[RangedSkill] TracerPrefab이 null");
                return;
            }

            TrailRenderer tracer = Object.Instantiate(TracerPrefab, origin, Quaternion.identity);
            tracer.AddPosition(origin);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity, TargetLayer))
            {
                tracer.transform.position = hit.point;
                Debug.Log($"[RangedSkill] 레이캐스트 히트: {hit.transform.name}");

                if (hit.transform.TryGetComponent<Player.Player>(out var player))
                    player.GetModule<AgentHealth>().ApplyDamage(Damage);
            }
            else
            {
                Debug.Log("[RangedSkill] 레이캐스트 미스");
                tracer.transform.position = origin + direction * 100f;
            }
        }

        private Transform FindMuzzle(Enemy enemy)
        {
            foreach (Transform t in enemy.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "Muzzle" || t.CompareTag("Muzzle"))
                {
                    Debug.Log($"[RangedSkill] Muzzle 발견: {t.name}");
                    return t;
                }
            }
            Debug.LogWarning("[RangedSkill] Muzzle 오브젝트를 찾지 못해 enemy.transform 사용");
            return enemy.transform;
        }
    }
}
