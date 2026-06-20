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
        [Tooltip("전체 퍼지는 각도 (예: 60이면 좌우 30도씩)")]
        [field: SerializeField] public float SpreadAngle { get; private set; } = 60f;

        [Header("투사체")]
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private float targetHeightOffset = 1f;

        private Transform _muzzle;
        private Transform _target;
        private EnemyAnimationEvent _animEvent;
        private EnemyVfxController _vfx;

        public override void ExecuteSkill(Enemy enemy)
        {
            _target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            _muzzle = FindMuzzle(enemy);
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();
            _vfx = enemy.GetModule<EnemyVfxController>();

            _animEvent.OnAttack += HandleAttack;
        }

        private void HandleAttack()
        {
            _animEvent.OnAttack -= HandleAttack;
            FireFan();
            NotifyComplete();
        }

        private void FireFan()
        {
            if (projectilePrefab == null || _muzzle == null) return;

            Vector3 centerDir = GetFireDirection();

            if (BulletCount <= 1)
            {
                SpawnBullet(centerDir);
                _vfx?.Play(EnemyVfxType.MuzzleFlash);
                return;
            }

            float halfAngle = SpreadAngle * 0.5f;
            float step = SpreadAngle / (BulletCount - 1);

            for (int i = 0; i < BulletCount; i++)
            {
                float angle = -halfAngle + step * i;
                // 월드 Y축 기준 회전: Y(고도)값은 그대로 보존되고 좌우(수평)만 퍼짐
                Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * centerDir;
                SpawnBullet(dir);
            }

            _vfx?.Play(EnemyVfxType.MuzzleFlash);
        }

        private void SpawnBullet(Vector3 direction)
        {
            // TODO: 풀링 적용 시 이 두 줄만 PoolManager.Get(...)으로 교체
            Projectile proj = Object.Instantiate(projectilePrefab, _muzzle.position, Quaternion.LookRotation(direction));
            proj.Init(direction, ProjectileSpeed, Damage, TargetLayer);
        }

        private Vector3 GetFireDirection()
        {
            if (_target != null)
            {
                Vector3 aimPoint = _target.position + Vector3.up * targetHeightOffset;
                return (aimPoint - _muzzle.position).normalized; // 높이값(기울기) 그대로 유지
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