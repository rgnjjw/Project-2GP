using System.Collections;
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
        [field: SerializeField] public float ShotInterval { get; private set; } = 0.1f;

        [Header("투사체")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float targetHeightOffset = 1f;

        private Transform _muzzle;
        private Transform _target;
        private EnemyAnimationEvent _animEvent;
        private EnemyVfxController _vfx;
        private Enemy _enemy;

        public override void ExecuteSkill(Enemy enemy)
        {
            _enemy = enemy;
            _target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            _muzzle = FindMuzzle(enemy);
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();
            _vfx = enemy.GetModule<EnemyVfxController>();

            _animEvent.OnAttack += HandleAttack;
        }

        private void HandleAttack()
        {
            _animEvent.OnAttack -= HandleAttack;
            _enemy.StartCoroutine(FireSequence());
        }

        private IEnumerator FireSequence()
        {
            for (int i = 0; i < ShotCount; i++)
            {
                FireOne();

                if (i < ShotCount - 1)
                    yield return new WaitForSeconds(ShotInterval);
            }

            NotifyComplete();
        }

        private void FireOne()
        {
            if (projectilePrefab == null || _muzzle == null) return;

            Vector3 direction = GetFireDirection();

            // TODO: 나중에 풀링 적용 시 이 두 줄만 PoolManager.Get(...)으로 교체
            Projectile proj = Object.Instantiate(projectilePrefab, _muzzle.position, Quaternion.LookRotation(direction));
            proj.Init(direction, ProjectileSpeed, Damage, TargetLayer);

            _vfx?.Play(EnemyVfxType.MuzzleFlash);
        }

        private Vector3 GetFireDirection()
        {
            if (_target != null)
            {
                Vector3 aimPoint = _target.position + Vector3.up * targetHeightOffset;
                return (aimPoint - _muzzle.position).normalized;
            }
            return _muzzle.forward;
        }

        private Transform FindMuzzle(Enemy enemy)
        {
            foreach (Transform t in enemy.GetComponentsInChildren<Transform>(true))
            {
                if (t.name == "Muzzle" || t.CompareTag("Muzzle"))
                    return t;
            }
            return enemy.transform;
        }
    }
}