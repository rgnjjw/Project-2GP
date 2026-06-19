using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class PiercingBullet : MonoBehaviour
    {
        private int _damage;
        private float _speed;
        private Vector3 _direction;
        private float _autoDeleteTime;
        private float _lifeTime;
        private LayerMask _hitMask;

        private readonly HashSet<Enemy.Enemy> _hitEnemies = new();

        public void Initialize(Vector3 direction, int damage, float speed, float autoDeleteTime, LayerMask hitMask)
        {
            _direction = direction.normalized;
            _damage = damage;
            _speed = speed;
            _autoDeleteTime = autoDeleteTime;
            _hitMask = hitMask;
            _lifeTime = 0f;
            _hitEnemies.Clear();
        }

        private void Update()
        {
            _lifeTime += Time.deltaTime;
            if (_lifeTime >= _autoDeleteTime)
            {
                Destroy(gameObject);
                return;
            }

            float stepDistance = _speed * Time.deltaTime;
            Ray ray = new Ray(transform.position, _direction);

            RaycastHit[] hits = Physics.RaycastAll(ray, stepDistance + 0.1f, _hitMask);
            if (hits.Length > 0)
            {
                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                foreach (var hit in hits)
                {
                    if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                    {
                        if (_hitEnemies.Add(enemy))
                        {
                            var health = enemy.GetModule<AgentHealth>();
                            if (health != null)
                            {
                                health.ApplyDamage(_damage);
                                EventBus.Publish(new PlayerDamageDealtEvent(_damage));
                            }
                        }
                    }
                    else
                    {
                        transform.position = hit.point;
                        Destroy(gameObject);
                        return;
                    }
                }
            }

            transform.position += _direction * stepDistance;
        }
    }
}
