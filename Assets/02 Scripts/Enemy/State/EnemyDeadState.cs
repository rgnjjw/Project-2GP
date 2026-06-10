using System;
using UnityEngine;

namespace _02_Scripts.Enemy.State
{
    public class EnemyDeadState : AbstractEnemyState
    {
        private readonly NavEnemyRenderer _navEnemyRenderer;
        private readonly EnemyAnimationEvent _animationEvent;
        public EnemyDeadState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            _navEnemyRenderer = agent.GetModule<NavEnemyRenderer>();
            _animationEvent = agent.GetModule<EnemyAnimationEvent>();
        }

        public override void Update()
        {
            base.Update();
            {
                Debug.Log("DEAD");
            }
        }

        public override void Enter(float crossFadeDuration, int layerIndex = 0)
        {
            base.Enter(crossFadeDuration, layerIndex);
            _animationEvent.OnDeath += HandleDeathEnd;
            _navEnemyRenderer.NavMeshAgent.ResetPath();
            _navEnemyRenderer.NavMeshAgent.velocity = Vector3.zero;
            _navEnemyRenderer.NavMeshAgent.enabled = false;

            _renderer.PlayClip(_stateClipHash, 0, 0.1f);
        }

        public override void Exit()
        {
            base.Exit();
            _animationEvent.OnDeath -= HandleDeathEnd;
        }

        private void HandleDeathEnd()
        {
            UnityEngine.Object.Destroy(enemy.gameObject);
        }
    }   
}