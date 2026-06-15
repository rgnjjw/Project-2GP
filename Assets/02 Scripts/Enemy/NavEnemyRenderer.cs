using _02_Scripts.Agent;
using _02_Scripts.Core.AnimationSystem;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;
using UnityEngine.AI;

namespace _02_Scripts.Enemy
{
    public class NavEnemyRenderer : AgentRenderer
    {
        [field: SerializeField] public NavMeshAgent NavMeshAgent { get; private set; }
        [field: SerializeField] public AnimParamSO SpeedAnimParam { get; private set; }

        [SerializeField] private float rotateSpeed = 360f;

        private Transform _enemyTrm;
        private Enemy _enemy;

        public bool IsRotationLocked { get; set; }
        public bool UseForcedRotation { get; set; }
        public bool UseChaseRotation { get; set; }

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            NavMeshAgent.updateRotation = false;
            if (owner is Enemy enemy)
            {
                _enemy = enemy;
                _enemyTrm = enemy.transform;
            }
        }

        private void Update()
        {
            if (IsRotationLocked) return;

            if (UseForcedRotation && _enemy.CurrentTarget != null)
                LookAtTarget(_enemy.CurrentTarget);
            else if (UseChaseRotation && _enemy.CurrentTarget != null)
                ChaseRotation(_enemy.CurrentTarget);
        }

        private void ChaseRotation(Transform target)
        {
            Vector3 velocity = NavMeshAgent.velocity;
            velocity.y = 0f;

            Vector3 toTarget = target.position - _enemyTrm.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude < 0.001f) return;

            float speedRatio = Mathf.Clamp01(NavMeshAgent.velocity.magnitude / NavMeshAgent.speed);

            Vector3 desiredDir = velocity.sqrMagnitude > 0.001f
                ? Vector3.Lerp(toTarget.normalized, velocity.normalized, speedRatio)
                : toTarget;

            if (desiredDir.sqrMagnitude < 0.001f) return;

            _enemyTrm.rotation = Quaternion.RotateTowards(
                _enemyTrm.rotation,
                Quaternion.LookRotation(desiredDir),
                rotateSpeed * Time.deltaTime
            );
        }

        private void LookAtTarget(Transform target)
        {
            Vector3 direction = target.position - _enemyTrm.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;
            _enemyTrm.rotation = Quaternion.RotateTowards(
                _enemyTrm.rotation,
                Quaternion.LookRotation(direction),
                rotateSpeed * Time.deltaTime
            );
        }
    }
}
