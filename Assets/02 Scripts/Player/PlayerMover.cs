using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerMover : AgentMover
    {
        [SerializeField] private float coyoteTime = 0.12f;

        [Header("Slope")]
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float slopeRayDistance = 1.5f;

        private float _lastGroundedTime = -999f;
        private Vector3 _slopeNormal = Vector3.up;

        public bool IsGroundedOrCoyote => IsGrounded || (Time.time - _lastGroundedTime <= coyoteTime);

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

            _rigidbody.linearVelocity = velocity;
            InvokeVelocityChanged(velocity);
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