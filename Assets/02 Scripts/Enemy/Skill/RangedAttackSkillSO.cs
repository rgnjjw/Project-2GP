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
        [field: SerializeField] public TrailRenderer TracerPrefab { get; private set; }
        [field: SerializeField] public LineRenderer LaserPrefab { get; private set; }

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

            if (_target != null)
            {
                Vector3 targetPos = _target.position + Vector3.up * TargetHeightOffset;
                Vector3 dir = (targetPos - _muzzle.position).normalized;
                FireTracer(_muzzle.position, dir);
            }

            _animEvent.OnPrepare -= HandlePrepare;
            _animEvent.OnAttack -= HandleAttack;

            NotifyComplete();
        }

        private void FireTracer(Vector3 origin, Vector3 direction)
        {
            if (TracerPrefab == null) return;

            TrailRenderer tracer = Object.Instantiate(TracerPrefab, origin, Quaternion.identity);
            tracer.AddPosition(origin);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity, TargetLayer))
            {
                tracer.transform.position = hit.point;

                if (hit.transform.TryGetComponent<Player.Player>(out var player))
                    player.GetModule<AgentHealth>().ApplyDamage(Damage);
            }
            else
            {
                tracer.transform.position = origin + direction * 100f;
            }
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