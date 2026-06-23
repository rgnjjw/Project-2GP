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

            transform.position += _direction * (_speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_initialized) return;

            // 대상 레이어(플레이어/벽 등)에 속한 것만 반응한다.
            if ((_targetLayer.value & (1 << other.gameObject.layer)) == 0) return;

            // 플레이어에 맞으면 피해를 준다(벽 등에 맞으면 피해 없이 사라짐).
            Player.Player player = other.GetComponentInParent<Player.Player>();
            if (player != null)
                player.GetModule<AgentHealth>()?.ApplyDamage(_damage);

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