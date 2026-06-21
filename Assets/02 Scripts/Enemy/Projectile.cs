using _02_Scripts.Agent;
using GGMLib.ObjectPool.Runtime;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class Projectile : AbstractMonoPoolable
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
                ReturnToPool();
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

            ReturnToPool();
        }

        // 풀이 초기화돼 있으면 풀로 반환하고, 아니면(에디터 테스트 등) 그냥 파괴한다.
        private void ReturnToPool()
        {
            _initialized = false;
            if (PoolManagerSO.Instance != null && Item != null)
                PoolManagerSO.Instance.Push(this);
            else
                Destroy(gameObject);
        }

        // 풀에서 재사용될 때 상태를 깨끗이 초기화. (Init이 곧 호출되지만 안전하게 리셋)
        public override void ResetItem()
        {
            _initialized = false;
            _elapsed = 0f;
        }
    }
}