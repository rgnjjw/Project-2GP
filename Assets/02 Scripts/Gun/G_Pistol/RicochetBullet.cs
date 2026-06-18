using _02_Scripts.Agent;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    // TODO: pool this projectile
    public class RicochetBullet : MonoBehaviour
    {
        private int _damage;
        private float _speed;
        private Vector3 _direction;
        private float _autoDeleteTime;
        private float _timeSinceLastBounce;
        private LayerMask _hitMask;

        public void Initialize(Vector3 direction, int damage, float speed, float autoDeleteTime, LayerMask hitMask)
        {
            _direction = direction.normalized;
            _damage = damage;
            _speed = speed;
            _autoDeleteTime = autoDeleteTime;
            _hitMask = hitMask;
            _timeSinceLastBounce = 0f;
        }

        private void Update()
        {
            _timeSinceLastBounce += Time.deltaTime;
            if (_timeSinceLastBounce >= _autoDeleteTime)
            {
                Destroy(gameObject);
                return;
            }

            float stepDistance = _speed * Time.deltaTime;
            Ray ray = new Ray(transform.position, _direction);

            if (Physics.Raycast(ray, out RaycastHit hit, stepDistance + 0.1f, _hitMask))
            {
                transform.position = hit.point;

                if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                {
                    var health = enemy.GetModule<AgentHealth>();
                    if (health != null)
                    {
                        health.ApplyDamage(_damage);
                        EventBus.Publish(new PlayerDamageDealtEvent(_damage));
                    }
                }

                _direction = Vector3.Reflect(_direction, hit.normal);
                _timeSinceLastBounce = 0f;
                transform.position += hit.normal * 0.05f;
            }
            else
            {
                transform.position += _direction * stepDistance;
            }
        }
    }
}
