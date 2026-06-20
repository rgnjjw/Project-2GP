using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 5f;

        private Vector3 _direction;
        private float _speed;
        private int _damage;
        private LayerMask _targetLayer;
        private float _elapsed;
        private bool _initialized;

        public void Init(Vector3 direction, float speed, int damage, LayerMask targetLayer)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _targetLayer = targetLayer;
            _elapsed = 0f;
            _initialized = true;

            transform.rotation = Quaternion.LookRotation(_direction);
        }

        private void Update()
        {
            if (!_initialized) return;

            _elapsed += Time.deltaTime;
            if (_elapsed >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            float moveDist = _speed * Time.deltaTime;

            if (Physics.Raycast(transform.position, _direction, out RaycastHit hit, moveDist, _targetLayer))
            {
                HandleHit(hit);
                return;
            }

            transform.position += _direction * moveDist;
        }

        private void HandleHit(RaycastHit hit)
        {
            if (hit.collider.TryGetComponent<Player.Player>(out var player))
                player.GetModule<AgentHealth>().ApplyDamage(_damage);

            Destroy(gameObject);
        }
    }
}