using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerMover : AgentMover
    {
        [Header("ULTRAKILL Physics")]
        [SerializeField] private float coyoteTime = 0.12f;

        private float _lastGroundedTime = -999f;

        public bool IsGroundedOrCoyote => IsGrounded || (Time.time - _lastGroundedTime <= coyoteTime);

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (IsGrounded) _lastGroundedTime = Time.time;
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

                if (_postDashVelocity.sqrMagnitude > target.sqrMagnitude + 0.01f)
                {
                    _postDashVelocity = Vector3.MoveTowards(_postDashVelocity, target, PostDashDecay * Time.fixedDeltaTime);
                    velocity.x = _postDashVelocity.x;
                    velocity.z = _postDashVelocity.z;
                }
                else
                {
                    _postDashVelocity = Vector3.zero;
                    velocity.x = targetX;
                    velocity.z = targetZ;
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
    }
}
