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

        [Tooltip("전체 퍼지는 각도. 360이면 원형 탄막. 예: BulletCount 8 + SpreadAngle 360 = 45도 간격 8방향.")]
        [field: SerializeField] public float SpreadAngle { get; private set; } = 60f;

        [Header("반복 발사")]
        [Tooltip("부채꼴 발사를 몇 번 반복할지. 1이면 애니메이션 1번, 부채꼴 1번 발사.")]
        [SerializeField] private int volleyCount = 1;

        [Header("애니메이션 반복")]
        [Tooltip("Animator State 이름. 예: Base Layer.MAGIC")]
        [SerializeField] private string attackStateName = "Base Layer.MAGIC";

        [Tooltip("공격 애니메이션이 있는 Animator Layer")]
        [SerializeField] private int animationLayer = 0;

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
            Animator animator = enemy.GetComponentInChildren<Animator>();

            if (animEvent == null || animator == null)
            {
                FireFan(muzzle, target, vfx);
                NotifyComplete();
                return;
            }

            int remainingVolleyCount = Mathf.Max(1, volleyCount);
            int attackStateHash = Animator.StringToHash(attackStateName);

            void HandleAttack()
            {
                FireFan(muzzle, target, vfx);
                remainingVolleyCount--;
            }

            void HandleAttackEnd()
            {
                if (remainingVolleyCount > 0)
                {
                    animator.Play(attackStateHash, animationLayer, 0f);
                    return;
                }

                animEvent.OnAttack -= HandleAttack;
                animEvent.OnAttackEnd -= HandleAttackEnd;

                NotifyComplete();
            }

            animEvent.OnAttack -= HandleAttack;
            animEvent.OnAttackEnd -= HandleAttackEnd;

            animEvent.OnAttack += HandleAttack;
            animEvent.OnAttackEnd += HandleAttackEnd;

            animator.Play(attackStateHash, animationLayer, 0f);
        }

        private void FireFan(Transform muzzle, Transform target, EnemyVfxController vfx)
        {
            if (projectilePrefab == null || muzzle == null)
                return;

            Vector3 centerDirection = GetFireDirection(muzzle, target);

            if (centerDirection.sqrMagnitude <= 0.0001f)
                centerDirection = muzzle.forward;

            centerDirection.Normalize();

            int bulletCount = Mathf.Max(1, BulletCount);
            float spreadAngle = Mathf.Max(0f, SpreadAngle);

            if (bulletCount <= 1)
            {
                SpawnBullet(muzzle, centerDirection);
                vfx?.Play(EnemyVfxType.MuzzleFlash);
                return;
            }

            if (spreadAngle >= 360f)
            {
                FireCircle(muzzle, centerDirection, bulletCount);
                vfx?.Play(EnemyVfxType.MuzzleFlash);
                return;
            }

            FireFanShape(muzzle, centerDirection, bulletCount, spreadAngle);
            vfx?.Play(EnemyVfxType.MuzzleFlash);
        }

        private void FireCircle(Transform muzzle, Vector3 centerDirection, int bulletCount)
        {
            float step = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = step * i;
                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * centerDirection;

                SpawnBullet(muzzle, direction);
            }
        }

        private void FireFanShape(Transform muzzle, Vector3 centerDirection, int bulletCount, float spreadAngle)
        {
            float halfAngle = spreadAngle * 0.5f;
            float step = spreadAngle / (bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = -halfAngle + step * i;
                Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * centerDirection;

                SpawnBullet(muzzle, direction);
            }
        }

        private void SpawnBullet(Transform muzzle, Vector3 direction)
        {
            if (muzzle == null || projectilePrefab == null)
                return;

            if (direction.sqrMagnitude <= 0.0001f)
                direction = muzzle.forward;

            direction.Normalize();

            Projectile projectile = Object.Instantiate(
                projectilePrefab,
                muzzle.position,
                Quaternion.LookRotation(direction)
            );

            projectile.Init(direction, ProjectileSpeed, Damage, TargetLayer);
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