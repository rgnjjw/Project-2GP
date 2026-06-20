using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace _02_Scripts.Enemy.State
{
    public class EnemyDeadState : AbstractEnemyState
    {
        private const float FallbackDestroyDelay = 3f;

        private readonly NavEnemyRenderer _navEnemyRenderer;
        private readonly EnemyAnimationEvent _animationEvent;

        private bool _destroyRequested;
        private Coroutine _fallbackCoroutine;

        public EnemyDeadState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            _navEnemyRenderer = agent.GetModule<NavEnemyRenderer>();
            _animationEvent = agent.GetModule<EnemyAnimationEvent>();
        }

        public override void Enter(float crossFadeDuration, int layerIndex = 0)
        {
            base.Enter(crossFadeDuration, layerIndex);

            _destroyRequested = false;

            if (_animationEvent != null)
            {
                _animationEvent.OnDeath -= HandleDeathEnd;
                _animationEvent.OnDeath += HandleDeathEnd;
            }

            StopNavMeshAgentSafely();

            _renderer.PlayClip(_stateClipHash, layerIndex, crossFadeDuration);

            if (_fallbackCoroutine != null)
                enemy.StopCoroutine(_fallbackCoroutine);

            _fallbackCoroutine = enemy.StartCoroutine(FallbackDestroy());
        }

        public override void Exit()
        {
            base.Exit();

            if (_animationEvent != null)
                _animationEvent.OnDeath -= HandleDeathEnd;

            if (_fallbackCoroutine != null && enemy != null)
            {
                enemy.StopCoroutine(_fallbackCoroutine);
                _fallbackCoroutine = null;
            }
        }

        private void StopNavMeshAgentSafely()
        {
            if (_navEnemyRenderer == null)
                return;

            NavMeshAgent navMeshAgent = _navEnemyRenderer.NavMeshAgent;

            if (navMeshAgent == null)
                return;

            if (navMeshAgent.isActiveAndEnabled && navMeshAgent.isOnNavMesh)
            {
                navMeshAgent.isStopped = true;

                if (navMeshAgent.hasPath)
                    navMeshAgent.ResetPath();

                navMeshAgent.velocity = Vector3.zero;
            }

            navMeshAgent.enabled = false;
        }

        private void HandleDeathEnd()
        {
            DestroyEnemy();
        }

        private IEnumerator FallbackDestroy()
        {
            yield return new WaitForSeconds(FallbackDestroyDelay);

            DestroyEnemy();
        }

        private void DestroyEnemy()
        {
            if (_destroyRequested)
                return;

            _destroyRequested = true;

            if (_animationEvent != null)
                _animationEvent.OnDeath -= HandleDeathEnd;

            if (_fallbackCoroutine != null && enemy != null)
            {
                enemy.StopCoroutine(_fallbackCoroutine);
                _fallbackCoroutine = null;
            }

            if (enemy != null)
                Object.Destroy(enemy.gameObject);
        }
    }
}