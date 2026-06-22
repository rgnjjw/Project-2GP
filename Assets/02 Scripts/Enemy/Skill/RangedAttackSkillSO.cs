using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "RangedAttackSkillSO", menuName = "Skill/RangedAttackSkillSO", order = 0)]
    public class RangedAttackSkillSO : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }
        [field: SerializeField] public float TargetHeightOffset { get; private set; } = 1.0f;
        [field: SerializeField] public LineRenderer LaserPrefab { get; private set; }

        [Header("Bullet Beam")]
        [SerializeField] private Material beamMaterial;
        [SerializeField] private float beamWidth = 0.02f;
        [SerializeField] private float beamDuration = 0.08f;

        public override void ExecuteSkill(Enemy enemy)
        {
            if (enemy == null)
                return;

            Transform target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            Transform muzzle = FindMuzzle(enemy);
            EnemyAnimationEvent animEvent = enemy.GetModule<EnemyAnimationEvent>();
            EnemyLaserAimer laserAimer = enemy.GetModule<EnemyLaserAimer>();
            EnemyVfxController vfx = enemy.GetModule<EnemyVfxController>();

            if (animEvent == null)
            {
                FireByCurrentMuzzleDirection(muzzle, vfx);
                NotifyComplete(enemy);
                return;
            }

            void HandlePrepare()
            {
                animEvent.OnPrepare -= HandlePrepare;

                if (LaserPrefab != null && laserAimer != null && muzzle != null)
                    laserAimer.StartAim(LaserPrefab, muzzle, target, TargetLayer);
            }

            void HandleAttack()
            {
                animEvent.OnPrepare -= HandlePrepare;
                animEvent.OnAttack -= HandleAttack;

                laserAimer?.StopAim();

                FireByCurrentMuzzleDirection(muzzle, vfx);

                NotifyComplete(enemy);
            }

            animEvent.OnPrepare += HandlePrepare;
            animEvent.OnAttack += HandleAttack;
        }

        private void FireByCurrentMuzzleDirection(Transform muzzle, EnemyVfxController vfx)
        {
            if (muzzle == null)
                return;

            FireBeam(muzzle, muzzle.position, muzzle.forward);
            vfx?.Play(EnemyVfxType.MuzzleFlash);
        }

        private void FireBeam(Transform muzzle, Vector3 origin, Vector3 direction)
        {
            if (muzzle == null)
                return;

            if (direction.sqrMagnitude <= 0.0001f)
                direction = muzzle.forward;

            direction.Normalize();

            Vector3 endPoint = origin + direction * 100f;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity, TargetLayer))
            {
                endPoint = hit.point;

                if (hit.transform.TryGetComponent<Player.Player>(out Player.Player player))
                {
                    AgentHealth health = player.GetModule<AgentHealth>();

                    if (health != null)
                        health.ApplyDamage(Damage);
                }
            }

            EnemyBulletBeamSKillSO beam = GetOrCreateBeam(muzzle);
            beam.Show(origin, endPoint, beamDuration);
        }

        private EnemyBulletBeamSKillSO GetOrCreateBeam(Transform muzzle)
        {
            EnemyBulletBeamSKillSO beam = muzzle.GetComponentInChildren<EnemyBulletBeamSKillSO>();

            if (beam == null)
            {
                GameObject beamObject = new GameObject("BulletBeam");
                beamObject.transform.SetParent(muzzle, false);

                beam = beamObject.AddComponent<EnemyBulletBeamSKillSO>();
                beam.Setup(beamMaterial, beamWidth);
            }

            return beam;
        }

        private Transform FindMuzzle(Enemy enemy)
        {
            if (enemy == null)
                return null;

            foreach (Transform child in enemy.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "Muzzle" || child.CompareTag("Muzzle"))
                    return child;
            }

            return enemy.transform;
        }
    }
}