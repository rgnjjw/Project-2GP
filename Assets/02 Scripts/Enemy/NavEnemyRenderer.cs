using System;
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
        
        [SerializeField] private float rotateSpeed;
        [SerializeField] private bool useForcedRotation;
        [SerializeField] private bool useNavRotation;

        private Transform _enemyTrm;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            
            NavMeshAgent.updateRotation = useNavRotation;
            if (owner is Enemy enemy)
                _enemyTrm = enemy.transform;
        }
        
        private void Update()
        {
            if(useForcedRotation)
                ForceRotationControl();
        }

        public void LookAtTarget(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0;
            if (direction == Vector3.zero) return;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }
        
        private void ForceRotationControl()
        {
            if (NavMeshAgent.remainingDistance < 0.01f) return;

            Vector3 desiredDirection = NavMeshAgent.steeringTarget - transform.position;
            if (desiredDirection.magnitude < 0.01f) return;

            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            _enemyTrm.rotation = Quaternion.RotateTowards(
                _enemyTrm.rotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
        }
        
    }
}