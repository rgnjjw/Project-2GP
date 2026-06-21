using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class EnemyGravityForceController : MonoBehaviour, IModule, IAfterInitModule
    {
        [Header("발동 조건")]
        [Tooltip("HP가 이 비율 이하로 떨어지면 중력장 발동. 0.2 = 20%")]
        [SerializeField] private float triggerHpRatio = 0.2f;

        [Header("재발동")]
        [SerializeField] private bool canRetrigger = false;
        [SerializeField] private float retriggerCooldown = 10f;

        [Header("대상 탐색")]
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private float targetSearchRadius = 20f;

        [Header("끌어당기기 / 밀어내기")]
        [SerializeField] private float forceDuration = 1.5f;

        [Tooltip("양수면 끌어당김, 음수면 밀어냄")]
        [SerializeField] private float forceSpeed = 8f;

        [Tooltip("끌어당기기일 때만 적용. 이 거리보다 가까워지면 즉시 종료하고 폭발")]
        [SerializeField] private float minDistance = 1.5f;

        [Header("종료 시 폭발")]
        [SerializeField] private float explosionRadius = 3f;
        [SerializeField] private int explosionDamage = 25;

        private Enemy _enemy;
        private AgentHealth _health;
        private EnemyVfxController _vfx;

        private bool _isRunning;
        private bool _hasTriggeredOnce;
        private float _lastTriggerTime = float.MinValue;
        private Coroutine _forceRoutine;

        public void Initialize(ModuleOwner owner)
        {
            _enemy = owner as Enemy;
            _health = owner.GetModule<AgentHealth>();
            _vfx = owner.GetModule<EnemyVfxController>();
        }

        public void AfterInit()
        {
            if (_health != null)
                _health.CurrentHp.OnValueChanged += OnHpChanged;
        }

        private void OnDestroy()
        {
            if (_health != null)
                _health.CurrentHp.OnValueChanged -= OnHpChanged;

            if (_forceRoutine != null)
            {
                StopCoroutine(_forceRoutine);
                _forceRoutine = null;
            }
        }

        private void OnHpChanged(int before, int current)
        {
            if (_isRunning) return;
            if (current <= 0) return;
            if (_health == null || _health.MaxHp <= 0) return;

            float hpRatio = (float)current / _health.MaxHp;

            if (hpRatio > triggerHpRatio)
                return;

            if (!CanTrigger())
                return;

            _forceRoutine = StartCoroutine(GravityForceRoutine());
        }

        private bool CanTrigger()
        {
            if (!_hasTriggeredOnce)
                return true;

            if (!canRetrigger)
                return false;

            return Time.time >= _lastTriggerTime + retriggerCooldown;
        }

        private IEnumerator GravityForceRoutine()
        {
            _isRunning = true;
            _hasTriggeredOnce = true;
            _lastTriggerTime = Time.time;

            Transform target = FindTarget();

            if (target == null)
            {
                EndForce();
                yield break;
            }

            Player.Player player = target.GetComponent<Player.Player>();

            if (player == null)
                player = target.GetComponentInParent<Player.Player>();

            AgentMover mover = player != null ? player.GetModule<AgentMover>() : null;

            if (player == null || mover == null)
            {
                EndForce();
                yield break;
            }

            bool isPulling = forceSpeed > 0f;
            float elapsed = 0f;

            _vfx?.Play(EnemyVfxType.GravityZone);
            mover.BeginSlide(Vector3.zero);

            while (elapsed < forceDuration)
            {
                if (_enemy == null || player == null)
                    break;

                elapsed += Time.fixedDeltaTime;

                Vector3 toEnemy = _enemy.transform.position - player.transform.position;
                toEnemy.y = 0f;

                float distance = toEnemy.magnitude;

                if (isPulling && distance <= minDistance)
                    break;

                Vector3 direction = distance > 0.0001f ? toEnemy.normalized : Vector3.zero;
                Vector3 forceVelocity = direction * forceSpeed;

                mover.UpdateSlideVelocity(forceVelocity);

                yield return new WaitForFixedUpdate();
            }

            mover.EndSlide();

            Explode(player);

            _vfx?.Stop(EnemyVfxType.GravityZone);
            _vfx?.Play(EnemyVfxType.Explosion);

            EndForce();
        }

        private void EndForce()
        {
            _isRunning = false;
            _forceRoutine = null;
        }

        private Transform FindTarget()
        {
            if (_enemy == null)
                return null;

            if (_enemy.CurrentTarget != null)
                return _enemy.CurrentTarget;

            Collider[] hits = Physics.OverlapSphere(
                _enemy.transform.position,
                targetSearchRadius,
                targetLayer,
                QueryTriggerInteraction.Ignore
            );

            Transform closestTarget = null;
            float closestDistanceSqr = float.MaxValue;

            foreach (Collider hit in hits)
            {
                if (!hit.TryGetComponent<Player.Player>(out _))
                    continue;

                float distanceSqr = (hit.transform.position - _enemy.transform.position).sqrMagnitude;

                if (distanceSqr >= closestDistanceSqr)
                    continue;

                closestDistanceSqr = distanceSqr;
                closestTarget = hit.transform;
            }

            return closestTarget;
        }

        private void Explode(Player.Player fallbackPlayer)
        {
            if (_enemy == null)
                return;

            Collider[] hits = Physics.OverlapSphere(
                _enemy.transform.position,
                explosionRadius,
                targetLayer,
                QueryTriggerInteraction.Ignore
            );

            HashSet<Player.Player> damagedPlayers = new();

            foreach (Collider hit in hits)
            {
                Player.Player player = hit.GetComponent<Player.Player>();

                if (player == null)
                    player = hit.GetComponentInParent<Player.Player>();

                if (player == null)
                    continue;

                if (!damagedPlayers.Add(player))
                    continue;

                AgentHealth health = player.GetModule<AgentHealth>();

                if (health != null)
                    health.ApplyDamage(explosionDamage);
            }

            if (damagedPlayers.Count > 0)
                return;

            if (fallbackPlayer == null)
                return;

            AgentHealth fallbackHealth = fallbackPlayer.GetModule<AgentHealth>();

            if (fallbackHealth != null)
                fallbackHealth.ApplyDamage(explosionDamage);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, targetSearchRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
#endif
    }
}