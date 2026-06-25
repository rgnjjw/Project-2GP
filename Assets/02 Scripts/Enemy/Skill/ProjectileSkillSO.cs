using GGMLib.ObjectPool.Runtime;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "ProjectileSkillSO", menuName = "Skill/ProjectileSkillSO", order = 0)]
    public class ProjectileSkillSO : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; } = 10;
        [field: SerializeField] public float ProjectileSpeed { get; private set; } = 20f;
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }

        [Header("연속 발사")]
        [field: SerializeField] public int ShotCount { get; private set; } = 3;

        [Header("애니메이션 반복")]
        [Tooltip("MAGIC 같은 Animator State 이름. Animator State 이름과 정확히 같아야 함.")]
        [SerializeField] private string attackStateName = "MAGIC";

        [Tooltip("공격 애니메이션이 들어있는 Animator Layer")]
        [SerializeField] private int animationLayer = 0;

        [Header("투사체")]
        [SerializeField] private Projectile projectilePrefab;
        [Tooltip("풀링용 아이템. 지정하면 풀에서 가져오고, 비우면 projectilePrefab을 Instantiate한다.")]
        [SerializeField] private PoolItemSO projectileItem;
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
                FireOne(muzzle, target, vfx);
                NotifyComplete(enemy);
                return;
            }

            int remainingShots = Mathf.Max(1, ShotCount);
            int attackStateHash = Animator.StringToHash(attackStateName);

            void HandleAttack()
            {
                FireOne(muzzle, target, vfx);
                remainingShots--;
            }

            void HandleAttackEnd()
            {
                if (remainingShots > 0)
                {
                    animator.Play(attackStateHash, animationLayer, 0f);
                    return;
                }

                animEvent.OnAttack -= HandleAttack;
                animEvent.OnAttackEnd -= HandleAttackEnd;

                NotifyComplete(enemy);
            }

            animEvent.OnAttack -= HandleAttack;
            animEvent.OnAttackEnd -= HandleAttackEnd;

            animEvent.OnAttack += HandleAttack;
            animEvent.OnAttackEnd += HandleAttackEnd;

            animator.Play(attackStateHash, animationLayer, 0f);
        }

        private void FireOne(Transform muzzle, Transform target, EnemyVfxController vfx)
        {
            if (projectilePrefab == null || muzzle == null)
                return;

            Vector3 direction = GetFireDirection(muzzle, target);

            if (direction.sqrMagnitude <= 0.0001f)
                direction = muzzle.forward;

            direction.Normalize();

            Projectile projectile = SpawnProjectile(muzzle.position, Quaternion.LookRotation(direction));
            if (projectile == null)
                return;

            projectile.Init(direction, ProjectileSpeed, Damage, TargetLayer);

            vfx?.Play(EnemyVfxType.MuzzleFlash);
        }

        private Projectile SpawnProjectile(Vector3 position, Quaternion rotation)
        {
            if (projectileItem != null && PoolManagerSO.Instance != null)
            {
                Projectile pooled = PoolManagerSO.Instance.Pop<Projectile>(projectileItem);
                pooled.transform.SetPositionAndRotation(position, rotation);
                return pooled;
            }

            if (projectilePrefab == null)
                return null;

            return Object.Instantiate(projectilePrefab, position, rotation);
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