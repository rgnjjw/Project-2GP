using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class EnemyLaserAimer : MonoBehaviour, IModule
    {
        [SerializeField] private float targetHeightOffset = 1.0f;

        private LineRenderer _laser;
        private Transform _muzzle;
        private Transform _target;
        private LayerMask _layerMask;
        private bool _active;

        public void Initialize(ModuleOwner owner) { }

        public void StartAim(LineRenderer laserPrefab, Transform muzzle, Transform target, LayerMask layerMask)
        {
            _muzzle = muzzle;
            _target = target;
            _layerMask = layerMask;
            _active = true;

            _laser = Instantiate(laserPrefab, muzzle);
        }

        public void StopAim()
        {
            _active = false;
            if (_laser != null)
            {
                Destroy(_laser.gameObject);
                _laser = null;
            }
        }

        private void Update()
        {
            if (!_active || _laser == null || _target == null || _muzzle == null) return;

            Vector3 targetPos = _target.position + Vector3.up * targetHeightOffset;
            Vector3 direction = (targetPos - _muzzle.position).normalized;
            Vector3 endPos = Physics.Raycast(_muzzle.position, direction, out RaycastHit hit, Mathf.Infinity, _layerMask)
                ? hit.point
                : _muzzle.position + direction * 100f;

            _laser.SetPosition(0, _muzzle.position);
            _laser.SetPosition(1, endPos);
        }
    }
}