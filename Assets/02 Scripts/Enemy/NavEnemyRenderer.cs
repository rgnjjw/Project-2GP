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

        // 공격 예열 중(발사 직전까지) 매 프레임 즉시(스냅) 플레이어를 바라보게 하는 모드.
        // 발사 순간에 꺼서 그 방향으로 탄/빔을 고정한다.
        public bool UseInstantForcedRotation { get; set; }

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            NavMeshAgent.updateRotation = false;
            if (Animator != null)
            {
                // 적은 NavMeshAgent로 이동하므로 루트 모션이 켜져 있으면 애니메이터가 위치를 덮어써
                // NavMeshAgent 이동을 상쇄한다(달리기 애니메이션만 재생되고 제자리에 머무는 버그).
                // 어떤 모델을 쓰든 항상 끄도록 강제한다.
                Animator.applyRootMotion = false;

                // 적은 NavMeshAgent로 이동(루트 모션 X)하므로 화면 밖에선 애니메이터 평가를 생략해도 위치가 어긋나지 않는다.
                // 적이 많을 때 오프스크린 애니메이션 연산을 줄여 프레임을 확보한다.
                Animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            }
            if (owner is Enemy enemy)
            {
                _enemy = enemy;
                _enemyTrm = enemy.transform;
            }
        }

        private void Update()
        {
            // 에이전트가 NavMesh 밖에 있으면 이동/경로 탐색이 불가능해 제자리 달리기가 된다.
            // (런타임에 NavMesh가 늦게 구워지거나, 살짝 벗어난 위치에 스폰된 경우 대비)
            // 근처 NavMesh 위로 끌어다 붙여 자가 복구한다.
            if (NavMeshAgent.enabled && !NavMeshAgent.isOnNavMesh)
            {
                if (NavMesh.SamplePosition(NavMeshAgent.transform.position, out NavMeshHit hit, 8f, NavMesh.AllAreas))
                    NavMeshAgent.Warp(hit.position);
                return;
            }

            if (IsRotationLocked) return;

            if (UseInstantForcedRotation && _enemy.CurrentTarget != null)
                SnapLookAtTarget();
            else if (UseForcedRotation && _enemy.CurrentTarget != null)
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

        // 조준 시작 시 단 한 번 플레이어 방향으로 즉시 회전(스냅)하고 이후엔 추적하지 않는다.
        // → 발사 방향이 그 즉시 고정되어, 레이저로 예고된 직선을 플레이어가 피할 수 있다.
        public void SnapLookAtTarget()
        {
            if (_enemy == null || _enemy.CurrentTarget == null) return;
            Vector3 direction = _enemy.CurrentTarget.position - _enemyTrm.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;
            _enemyTrm.rotation = Quaternion.LookRotation(direction);
        }

        // 플레이어(CurrentTarget)가 아닌 임의의 위치를 향해 즉시 1회 회전(스냅).
        // 힐처럼 아군을 바라봐야 하는 스킬에서 사용한다.
        public void SnapLookAt(Vector3 worldPosition)
        {
            if (_enemyTrm == null) return;
            Vector3 direction = worldPosition - _enemyTrm.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.001f) return;
            _enemyTrm.rotation = Quaternion.LookRotation(direction);
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
