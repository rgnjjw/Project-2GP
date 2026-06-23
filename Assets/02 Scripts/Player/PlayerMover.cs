using System;
using _02_Scripts.Agent;
using _02_Scripts.Core.ModuleSystem;
using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerMover : AgentMover
    {
        [SerializeField] private float coyoteTime = 0.12f;

        [Header("Slope")]
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float slopeRayDistance = 1.5f;

        [Header("Wall Slide (벽 끼임/기어오르기 방지)")]
        [Tooltip("벽 미끄러짐 처리를 켤지")]
        [SerializeField] private bool enableWallSlide = true;
        [Tooltip("벽 판정에 쓸 레이어. 보통 바닥/맵과 동일하게.")]
        [SerializeField] private LayerMask wallMask = ~0;
        [Tooltip("캡슐 앞쪽으로 벽을 얼마나 미리 감지할지(여유 거리).")]
        [SerializeField] private float wallProbeDistance = 0.25f;
        [Tooltip("이 값보다 법선 y의 절댓값이 작으면 '벽/가파른 모서리'로 간주(0.7≈45도보다 가파름).")]
        [SerializeField] private float wallMaxNormalY = 0.7f;
        [Tooltip("벽 판정용 캡슐 콜라이더(보통 플레이어 루트의 CapsuleCollider).")]
        [SerializeField] private CapsuleCollider playerCollider;

        private float _lastGroundedTime = -999f;
        private Vector3 _slopeNormal = Vector3.up;

        // 스폰 직후 false→true 접지에서 착지음이 울리지 않도록, 한 번 공중에 뜬 뒤부터만 재생한다.
        private bool _hasLeftGround;

        public bool IsGroundedOrCoyote => IsGrounded || (Time.time - _lastGroundedTime <= coyoteTime);

        public override void Initialize(ModuleOwner moduleOwner)
        {
            base.Initialize(moduleOwner);
            OnGroundStatusChanged += HandleGroundStatusChanged;
        }

        private void OnDestroy()
        {
            OnGroundStatusChanged -= HandleGroundStatusChanged;
        }

        private void HandleGroundStatusChanged(bool isGrounded)
        {
            if (!isGrounded)
            {
                _hasLeftGround = true;
                return;
            }

            if (_hasLeftGround)
                SoundManager.Instance.PlaySound("Landing");
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsGrounded)
            {
                _lastGroundedTime = Time.time;
                DetectSlope();
            }
            else
            {
                _slopeNormal = Vector3.up;
            }
        }

        private void DetectSlope()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, slopeRayDistance))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle <= maxSlopeAngle)
                    _slopeNormal = hit.normal;
                else
                    _slopeNormal = Vector3.up;
            }
            else
            {
                _slopeNormal = Vector3.up;
            }
        }

        private Vector3 ProjectOnSlope(Vector3 direction)
        {
            return Vector3.ProjectOnPlane(direction, _slopeNormal).normalized * direction.magnitude;
        }

        protected override void MoveCharacter()
        {
            Vector3 velocity = _rigidbody.linearVelocity;

            if (_isSlideOverride)
            {
                velocity.x = _slideOverrideVelocity.x;
                velocity.z = _slideOverrideVelocity.z;
                _postDashVelocity = Vector3.zero;
            }
            else
            {
                float targetX = _movementX * _moveSpeed * _moveSpeedMultiplier;
                float targetZ = _movementZ * _moveSpeed * _moveSpeedMultiplier;
                Vector3 target = new Vector3(targetX, 0f, targetZ);

                bool onSlope = _slopeNormal != Vector3.up;
                if (onSlope)
                    target = ProjectOnSlope(target);

                if (_postDashVelocity.sqrMagnitude > target.sqrMagnitude + 0.01f)
                {
                    _postDashVelocity = Vector3.MoveTowards(_postDashVelocity, target, PostDashDecay * Time.fixedDeltaTime);
                    velocity.x = _postDashVelocity.x;
                    velocity.z = _postDashVelocity.z;
                    if (onSlope) velocity.y = _postDashVelocity.y;
                }
                else
                {
                    _postDashVelocity = Vector3.zero;
                    velocity.x = target.x;
                    velocity.z = target.z;
                    if (onSlope) velocity.y = target.y;
                }
            }

            // 벽에 박힐 때: 벽 면을 따라 미끄러지게 하고(전진 느려짐/끼임 해소),
            // 위로 향한 가파른 모서리에 눌려 기어오르는 것을 막는다.
            if (enableWallSlide)
            {
                Vector3 horiz = new Vector3(velocity.x, 0f, velocity.z);
                horiz = ApplyWallSlide(horiz, ref velocity);
                velocity.x = horiz.x;
                velocity.z = horiz.z;
            }

            _rigidbody.linearVelocity = velocity;
            InvokeVelocityChanged(velocity);
        }

        // 이동 방향 주변 여러 방향으로 캡슐을 쏘아 '모든' 벽 면을 찾고, 각 면으로 들어가는 수평 속도 성분을 제거.
        // 평면 벽 → 면을 따라 미끄러짐. 오목한 모서리(두 벽이 만나는 곳) → 양쪽 성분이 모두 빠져 속도 0
        //  → 누르는 힘이 사라져 코너에 끼이거나 버티지 못하고 중력으로 떨어진다.
        // 위로 향한 가파른 모서리에 눌릴 땐 상승 속도까지 제거해 기어오르기를 막는다.
        private Vector3 ApplyWallSlide(Vector3 horizVel, ref Vector3 velocity)
        {
            if (horizVel.sqrMagnitude < 0.0001f) return horizVel;

            // 캡슐 기준(없으면 트랜스폼 기준)으로 캐스트 원점/반지름을 잡는다.
            Vector3 center;
            float radius;
            if (playerCollider != null)
            {
                Bounds b = playerCollider.bounds;
                center = b.center;
                radius = Mathf.Max(b.extents.x, b.extents.z) * 0.9f;
            }
            else
            {
                center = transform.position;
                radius = 0.4f;
            }

            Vector3 moveDir = horizVel.normalized;

            // 이동 방향을 중심으로 ±각도 부채꼴로 쏴, 코너의 양쪽 벽을 모두 감지한다.
            Span<float> fan = stackalloc float[] { 0f, 45f, -45f, 90f, -90f };

            bool climbBlock = false;

            // 같은 벽을 중복 제거하지 않도록 모은 노멀들에 대해 두 번 훑어 코너에서도 안정적으로 수렴시킨다.
            Span<Vector3> normals = stackalloc Vector3[5];
            int normalCount = 0;

            foreach (float angle in fan)
            {
                Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * moveDir;
                Vector3 origin = center - dir * radius; // 이미 붙어있어도 잡히게 살짝 뒤에서 출발

                if (!Physics.SphereCast(origin, radius, dir, out RaycastHit hit,
                        radius + wallProbeDistance, wallMask, QueryTriggerInteraction.Ignore))
                    continue;

                // 걸어 올라갈 수 있는 완만한 경사면은 벽이 아니다.
                if (Mathf.Abs(hit.normal.y) >= wallMaxNormalY) continue;

                // 위로 향한 가파른 모서리 → 기어오르기 차단 대상
                if (hit.normal.y > 0.1f) climbBlock = true;

                Vector3 nFlat = new Vector3(hit.normal.x, 0f, hit.normal.z);
                if (nFlat.sqrMagnitude < 0.0001f) continue;
                nFlat.Normalize();

                // 거의 같은 방향의 벽이면 건너뛴다(중복 제거).
                bool dup = false;
                for (int i = 0; i < normalCount; i++)
                    if (Vector3.Dot(normals[i], nFlat) > 0.95f) { dup = true; break; }
                if (!dup && normalCount < normals.Length) normals[normalCount++] = nFlat;
            }

            // 모은 모든 벽 면에 대해 두 번 반복 투영 → 코너에서도 양쪽 성분이 확실히 제거됨.
            for (int pass = 0; pass < 2; pass++)
                for (int i = 0; i < normalCount; i++)
                    if (Vector3.Dot(horizVel, normals[i]) < 0f)
                        horizVel = Vector3.ProjectOnPlane(horizVel, normals[i]);

            // 기어오르기 차단: 위로 향한 가파른 모서리에 눌려 공중에서 떠오르는 속도를 죽인다.
            // (법선 y가 거의 0인 '평평한 벽'은 건드리지 않아 벽점프/벽타기에 영향 없음)
            if (climbBlock && !IsGrounded && velocity.y > 0f)
                velocity.y = 0f;

            return horizVel;
        }

        public void EndSlideAndTransferMomentum()
        {
            Vector3 slideVelocity = _slideOverrideVelocity;
            EndSlide();
            float targetSpeedSq = (_movementX * _movementX + _movementZ * _movementZ)
                                  * _moveSpeed * _moveSpeed * _moveSpeedMultiplier * _moveSpeedMultiplier;
            if (slideVelocity.sqrMagnitude > targetSpeedSq + 0.01f)
                _postDashVelocity = slideVelocity;
        }

        public void SyncMomentumFromVelocity()
        {
            Vector3 v = _rigidbody.linearVelocity;
            _postDashVelocity = new Vector3(v.x, 0f, v.z);
        }

        public override void SetDashVelocity(Vector3 velocity, float duration)
        {
            if (_isSlideOverride) EndSlide();
            _postDashVelocity = velocity;
            Vector3 vel = _rigidbody.linearVelocity;
            vel.x = velocity.x;
            vel.z = velocity.z;
            _rigidbody.linearVelocity = vel;
        }

        public void ForceSetXZVelocity(Vector3 xzVelocity)
        {
            _postDashVelocity = xzVelocity;
            Vector3 vel = _rigidbody.linearVelocity;
            vel.x = xzVelocity.x;
            vel.z = xzVelocity.z;
            _rigidbody.linearVelocity = vel;
        }
    }
}