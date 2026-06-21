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

        // 매 프레임 Physics.RaycastAll은 호출마다 새 배열을 할당해 GC를 유발한다.
        // 총알마다 버퍼를 1회 만들어 재사용(RaycastNonAlloc)해 매 프레임 할당을 없앤다.
        private readonly RaycastHit[] _hitBuffer = new RaycastHit[32];

        private static readonly IComparer<RaycastHit> DistanceComparer = new RaycastHitDistanceComparer();

        private sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit a, RaycastHit b) => a.distance.CompareTo(b.distance);
        }

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

            int hitCount = Physics.RaycastNonAlloc(ray, _hitBuffer, stepDistance + 0.1f, _hitMask);
            if (hitCount > 0)
            {
                System.Array.Sort(_hitBuffer, 0, hitCount, DistanceComparer);

                for (int i = 0; i < hitCount; i++)
                {
                    RaycastHit hit = _hitBuffer[i];
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
