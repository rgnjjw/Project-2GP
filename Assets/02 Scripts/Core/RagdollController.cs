using _02_Scripts.Agent;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Core
{
    public class RagdollController : MonoBehaviour,IAfterInitModule
    {
        private AgentHealth _healthModule;
        private CapsuleCollider _ownerCollider;
        private Animator _animator;
        public void Initialize(ModuleOwner owner)
        {
            _animator = GetComponent<Animator>();
            _ownerCollider = owner.GetComponent<CapsuleCollider>();
            _healthModule = owner.GetModule<AgentHealth>();
        }

        public void AfterInit()
        {
            _healthModule.OnDead += OnDead;
        }
        private void OnEnable()
        {
            if (_healthModule == null)
                _healthModule.OnDead += OnDead;
        }

        private void OnDisable()
        {
            _healthModule.OnDead -= OnDead;
        }

        private void OnDead()
        {
            _ownerCollider.enabled = false;
            SetRigidbody(false);
            SetCollider(true);
            _animator.enabled = false;
            
        }

        public void ResetRagdoll()
        {
            _ownerCollider.enabled = true;
            SetRigidbody(true);
            SetCollider(false);
            _animator.enabled = true;
        }

        private void SetRigidbody(bool kinematicState)
        {
            foreach (var rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = kinematicState;
                if (!kinematicState)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;     
                }
            }
        }
        private void SetCollider(bool colliderState)
        {
            foreach (var collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = colliderState;
            }
        }

    }
}