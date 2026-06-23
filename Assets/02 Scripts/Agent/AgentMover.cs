using System;
using _02_Scripts.Agent.Interface;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public class AgentMover : MonoBehaviour,IModule, IMover
    {
        [Header("Agent Values")]
        [SerializeField] protected LayerMask whatIsGround;
        [SerializeField] private Vector3 groundCheckSize;
        protected float _moveSpeed;

        protected Rigidbody _rigidbody;
        private IMover _mover;
        private IAgentData _ownerData;

        protected float _movementX;
        protected float _movementZ;

        protected float _moveSpeedMultiplier;
        protected Vector3 _postDashVelocity;
        protected const float PostDashDecay = 80f;
        protected bool _isSlideOverride;
        protected Vector3 _slideOverrideVelocity;

        public bool IsGrounded {get; private set; }
        public event Action<bool> OnGroundStatusChanged;
        public event Action<Vector3> OnVelocityChanged;

        protected void InvokeVelocityChanged(Vector3 velocity) => OnVelocityChanged?.Invoke(velocity);

        public virtual void Initialize(ModuleOwner moduleOwner)
        {
            _rigidbody = moduleOwner.gameObject.GetComponent<Rigidbody>();
            _ownerData = moduleOwner.GetModule<IAgentData>();
            _moveSpeed = _ownerData.MoveSpeed.Value;
            _ownerData.MoveSpeed.OnValueChanged += SetMoveSpeed;
            _moveSpeedMultiplier = 1f;
        }


        public void SetMoveSpeedMultiplier(float value) => _moveSpeedMultiplier = value;

        public void SetVerticalVelocity(float yVelocity)
        {
            Vector3 vel = _rigidbody.linearVelocity;
            vel.y = yVelocity;
            _rigidbody.linearVelocity = vel;
        }

        public void BeginSlide(Vector3 initialVelocity)
        {
            _isSlideOverride = true;
            _slideOverrideVelocity = initialVelocity;
        }

        public void UpdateSlideVelocity(Vector3 velocity)
        {
            _slideOverrideVelocity = velocity;
        }

        public void EndSlide()
        {
            _isSlideOverride = false;
            _slideOverrideVelocity = Vector3.zero;
        }

        public virtual void SetDashVelocity(Vector3 velocity, float duration)
        {
            _postDashVelocity = velocity;
        }

        public virtual void AddForceToAgent(Vector3 force)
        {
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
        
        public void StopImmediately(bool xAxis, bool yAxis, bool zAxis)
        {
            Vector3 velocity = _rigidbody.linearVelocity;

            if (xAxis) velocity.x = 0;
            if (yAxis) velocity.y = 0;
            if (zAxis) velocity.z = 0;

            _rigidbody.linearVelocity = velocity;
        }
        protected void SetMoveSpeed(float before, float current)
        {
            _moveSpeed = current;
        }

        public void SetMovementX(float value)
        {
            _movementX = value;
        }

        public void SetMovementZ(float value)
        {
            _movementZ  = value;
        }

        protected virtual void FixedUpdate()
        {
            CheckGround();
            MoveCharacter();
        }

        protected void CheckGround()
        {
            bool before = IsGrounded;
            // CheckBox는 bool만 반환해 OverlapBox와 달리 배열을 할당하지 않는다(매 FixedUpdate·에이전트마다 GC 방지).
            // 트리거 콜라이더는 접지로 치지 않는다(트리거 통과 시 잘못 접지 판정 → 감속/이상 동작 방지).
            IsGrounded = Physics.CheckBox(transform.position, groundCheckSize, Quaternion.identity, whatIsGround,
                QueryTriggerInteraction.Ignore);
            if (before != IsGrounded)
            {
                OnGroundStatusChanged?.Invoke(IsGrounded);
            }
        }

        protected virtual void MoveCharacter()
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

            OnVelocityChanged?.Invoke(velocity);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, groundCheckSize);
        }
    }
}