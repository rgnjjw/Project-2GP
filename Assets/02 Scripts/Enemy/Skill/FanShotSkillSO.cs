using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "FanShotSkillSO", menuName = "Skill/FanShotSkillSO", order = 0)]
    public class FanShotSkillSO : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; } = 10;
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 20f;
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }

        [Header("부채꼴")]
        [field: SerializeField] public int BulletCount { get; private set; } = 5;

        [Tooltip("전체 퍼지는 각도. 예: 60이면 좌우 30도씩")]
        [field: SerializeField] public float SpreadAngle { get; private set; } = 60f;

        [Header("투사체")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float targetHeightOffset = 1f;

        public override void ExecuteSkill(Enemy enemy)
        {
            if (enemy == null)
                return;

            Transform target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            Transform muzzle = FindMuzzle(enemy);
            EnemyAnimationEvent animEvent = enemy.GetModule<EnemyAnimationEvent>();
            EnemyVfxController vfx = enemy.GetModule<EnemyVfxController>();

            if (animEvent == null)
            {
                FireFan(muzzle, target, vfx);
                NotifyComplete();
                return;
            }

            void HandleAttack()
            {
                animEvent.OnAttack -= HandleAttack;

                FireFan(muzzle, target, vfx);
                NotifyComplete();
            }

            animEvent.OnAttack += HandleAttack;
        }

        private void FireFan(Transform muzzle, Transform target, EnemyVfxController vfx)
        {
            if (projectilePrefab == null || muzzle == null)
                return;

            Vector3 centerDirection = GetFireDirection(muzzle, target);

            if (BulletCount <= 1)
            {
                SpawnBullet(muzzle, centerDirection);
                vfx?.Play(EnemyVfxType.MuzzleFlash);
                return;
            }

            float halfAngle = SpreadAngle * 0.5f;
            float step = SpreadAngle / (BulletCount - 1);

            for (int i = 0; i < BulletCount; i++)
            {
                float angle = -halfAngle + step * i;
                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * centerDirection;

                SpawnBullet(muzzle, direction);
            }

            vfx?.Play(EnemyVfxType.MuzzleFlash);
        }

        private void SpawnBullet(Transform muzzle, Vector3 direction)
        {
            if (muzzle == null || projectilePrefab == null)
                return;

            if (direction.sqrMagnitude <= 0.0001f)
                direction = muzzle.forward;

            Projectile projectile = Object.Instantiate(
                projectilePrefab,
                muzzle.position,
                Quaternion.LookRotation(direction.normalized)
            );

            projectile.Init(direction.normalized, ProjectileSpeed, Damage, TargetLayer);
        }

        private Vector3 GetFireDirection(Transform muzzle, Transform target)
        {
            if (muzzle == null)
                return Vector3.forward;

            if (target != null)
            {
                Vector3 aimPoint = target.position + Vector3.up * targetHeightOffset;
                Vector3 direction = aimPoint - muzzle.position;

                if (direction.sqrMagnitude > 0.0001f)
                    return direction.normalized;
            }

            return muzzle.forward;
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