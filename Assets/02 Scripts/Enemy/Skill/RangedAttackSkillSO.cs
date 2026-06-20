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

        private Transform _muzzle;
        private Transform _target;
        private EnemyAnimationEvent _animEvent;
        private EnemyLaserAimer _laserAimer;

        public override void ExecuteSkill(Enemy enemy)
        {
            _target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            _muzzle = FindMuzzle(enemy);
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();
            _laserAimer = enemy.GetModule<EnemyLaserAimer>();

            _animEvent.OnPrepare += HandlePrepare;
            _animEvent.OnAttack += HandleAttack;
        }

        private void HandlePrepare()
        {
            if (LaserPrefab != null && _laserAimer != null)
                _laserAimer.StartAim(LaserPrefab, _muzzle, _target, TargetLayer);
        }

        private void HandleAttack()
        {
            _laserAimer?.StopAim();

            // 플레이어 위치로 재조준하지 않고, 조준 단계에서 회전이 멈춘 그대로의 총구 정면 방향으로 발사한다.
            if (_muzzle != null)
                FireBeam(_muzzle.position, _muzzle.forward);

            _animEvent.OnPrepare -= HandlePrepare;
            _animEvent.OnAttack -= HandleAttack;

            NotifyComplete();
        }

        private void FireBeam(Vector3 origin, Vector3 direction)
        {
            Vector3 endPoint = origin + direction * 100f;
            if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity, TargetLayer))
            {
                endPoint = hit.point;
                if (hit.transform.TryGetComponent<Player.Player>(out var player))
                    player.GetModule<AgentHealth>().ApplyDamage(Damage);
            }

            EnemyBulletBeam beam = GetOrCreateBeam(_muzzle);
            beam.Show(origin, endPoint, beamDuration);
        }

        private EnemyBulletBeam GetOrCreateBeam(Transform muzzle)
        {
            var beam = muzzle.GetComponentInChildren<EnemyBulletBeam>();
            if (beam == null)
            {
                var go = new GameObject("BulletBeam");
                go.transform.SetParent(muzzle);
                beam = go.AddComponent<EnemyBulletBeam>();
                beam.Setup(beamMaterial, beamWidth);
            }
            return beam;
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
