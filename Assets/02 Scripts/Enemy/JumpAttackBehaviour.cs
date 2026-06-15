using System;
using System.Collections;
using _02_Scripts.Agent;
using _02_Scripts.Enemy.Skill;
using UnityEngine;
using UnityEngine.AI;

namespace _02_Scripts.Enemy
{
    public class JumpAttackBehaviour : MonoBehaviour
    {
        private JumpAttackSkillSO _data;
        private Enemy _enemy;
        private Action _onComplete;
        private NavEnemyRenderer _navEnemyRenderer;

        public void Execute(Enemy enemy, Vector3 targetPos, JumpAttackSkillSO data, Action onComplete)
        {
            _enemy = enemy;
            _data = data;
            _onComplete = onComplete;
            _navEnemyRenderer = enemy.GetModule<NavEnemyRenderer>();
            StartCoroutine(JumpRoutine(targetPos));
        }

        private IEnumerator JumpRoutine(Vector3 targetPos)
        {
            NavMeshAgent navAgent = _navEnemyRenderer.NavMeshAgent;

            Rigidbody rb = _enemy.GetComponent<Rigidbody>();
            bool originalKinematic = false;
            if (rb != null)
            {
                originalKinematic = rb.isKinematic;
                rb.isKinematic = true;
            }

            navAgent.enabled = false;
            _navEnemyRenderer.IsRotationLocked = true;

            if (_data.JumpAnimParam != null)
                _navEnemyRenderer.PlayClip(_data.JumpAnimParam.ParamHash, 0, 0.1f);

            Vector3 startPos = _enemy.transform.position;
            float elapsed = 0f;
            bool airAnimPlayed = false;

            while (elapsed < _data.JumpDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / _data.JumpDuration);

                Vector3 flat = Vector3.Lerp(startPos, targetPos, t);
                float arc = Mathf.Sin(t * Mathf.PI) * _data.JumpHeight;
                _enemy.transform.position = new Vector3(flat.x, flat.y + arc, flat.z);

                if (!airAnimPlayed && t >= 0.5f)
                {
                    airAnimPlayed = true;
                    if (_data.AirAnimParam != null)
                        _navEnemyRenderer.PlayClip(_data.AirAnimParam.ParamHash, 0, 0.1f);
                }

                yield return null;
            }

            _enemy.transform.position = targetPos;

            if (_data.LandingAnimParam != null)
                _navEnemyRenderer.PlayClip(_data.LandingAnimParam.ParamHash, 0, 0.1f);

            ApplyBlast(targetPos);

            navAgent.enabled = true;
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                navAgent.Warp(hit.position);

            if (rb != null)
                rb.isKinematic = originalKinematic;

            _navEnemyRenderer.IsRotationLocked = false;

            Destroy(this);
            _onComplete?.Invoke();
        }

        private void ApplyBlast(Vector3 center)
        {
            var cols = Physics.OverlapSphere(center, _data.DamageAreaDetection.Range);
            foreach (var col in cols)
            {
                if (col.transform == _enemy.transform) continue;
                if (col.TryGetComponent<Player.Player>(out var player))
                    player.GetModule<AgentHealth>().ApplyDamage(_data.Damage);
            }
        }
    }
}
