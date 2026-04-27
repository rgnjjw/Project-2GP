using System;
using _02_Scripts.Agent.Interface;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public class AgentMover : MonoBehaviour,IModule, IMover
    {
        [Header("Agent Values")]
        [SerializeField] private LayerMask whatIsGround;
        [SerializeField] private Vector3 groundCheckSize;
        private float _moveSpeed;
        
        private Rigidbody _rigidbody;
        private IMover _mover;
        private IAgentData _ownerData;
        
        private float _movementX;
        private float _movementZ;
        
        private float _moveSpeedMultiplier;

        public bool IsGrounded {get; private set; }
        public event Action<bool> OnGroundStatusChanged;
        public event Action<Vector3> OnVelocityChanged;

        public virtual void Initialize(ModuleOwner moduleOwner)
        {
            _rigidbody = moduleOwner.gameObject.GetComponent<Rigidbody>();
            _ownerData = moduleOwner.GetModule<IAgentData>();
            _moveSpeed = _ownerData.MoveSpeed.Value;
            _ownerData.MoveSpeed.OnValueChanged += SetMoveSpeed;
            _moveSpeedMultiplier = 1f;
        }


        public void SetMoveSpeedMultiplier(float value) => _moveSpeedMultiplier = value;

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
        private void SetMoveSpeed(float before, float current)
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

        private void FixedUpdate()
        {
            CheckGround();
            MoveCharacter();
        }

        private void CheckGround()
        {
            bool before = IsGrounded;
            IsGrounded = Physics.OverlapBox(transform.position, groundCheckSize, Quaternion.identity, whatIsGround)
                .Length > 0;
            if (before != IsGrounded)
            {
                OnGroundStatusChanged?.Invoke(IsGrounded);
            }
        }

        private void MoveCharacter()
        {
            Vector3 velocity = _rigidbody.linearVelocity;

            velocity.x = _movementX * _moveSpeed * _moveSpeedMultiplier;
            velocity.z = _movementZ * _moveSpeed * _moveSpeedMultiplier;

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