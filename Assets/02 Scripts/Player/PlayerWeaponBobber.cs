using System.Collections.Generic;
using _02_Scripts.Agent.Interface;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerWeaponBobber : MonoBehaviour, IModule, IAfterInitModule
    {
        [Header("Bob Settings")]
        [SerializeField] private float bobFrequency = 8f;
        [SerializeField] private float bobAmplitude = 0.04f;
        [SerializeField] private float bobReturnSpeed = 10f;

        private Player _player;
        private PlayerMover _playerMover;
        private PlayerVisualController _visualController;

        private Dictionary<PlayerRenderer, Vector3> _baseLocalPositions;
        private PlayerRenderer _previousVisual;

        private Vector3 _currentVelocity;
        private float _maxSpeed;
        private float _bobTime;
        private float _currentBobY;

        public void Initialize(ModuleOwner owner)
        {
            _player = owner as Player;
        }

        public void AfterInit()
        {
            _playerMover = _player.GetModule<PlayerMover>();
            _visualController = _player.GetModule<PlayerVisualController>();
            _maxSpeed = _player.GetModule<IAgentData>().MoveSpeed.Value;

            _baseLocalPositions = new Dictionary<PlayerRenderer, Vector3>();
            foreach (var visual in _player.GetModules<PlayerRenderer>())
                _baseLocalPositions[visual] = visual.transform.localPosition;

            _playerMover.OnVelocityChanged += OnVelocityChanged;
        }

        private void OnVelocityChanged(Vector3 velocity)
        {
            _currentVelocity = velocity;
        }

        private void Update()
        {
            if (_visualController == null || _playerMover == null) return;

            PlayerRenderer currentVisual = _visualController.CurrentVisual;

            // 비주얼이 바뀌면 이전 비주얼 위치 복원 + bob 상태 리셋
            if (currentVisual != _previousVisual)
            {
                if (_previousVisual != null && _baseLocalPositions.TryGetValue(_previousVisual, out var oldBase))
                    _previousVisual.transform.localPosition = oldBase;

                _currentBobY = 0f;
                _bobTime = 0f;
                _previousVisual = currentVisual;
            }

            if (currentVisual == null) return;
            if (!_baseLocalPositions.TryGetValue(currentVisual, out Vector3 basePos)) return;

            float xzSpeed = new Vector3(_currentVelocity.x, 0f, _currentVelocity.z).magnitude;
            float speedRatio = Mathf.Clamp01(xzSpeed / _maxSpeed);

            if (speedRatio > 0.01f)
                _bobTime += Time.deltaTime * bobFrequency * speedRatio;

            float targetBobY = Mathf.Sin(_bobTime * Mathf.PI * 2f) * bobAmplitude * speedRatio;
            _currentBobY = Mathf.Lerp(_currentBobY, targetBobY, Time.deltaTime * bobReturnSpeed);

            Vector3 newPos = basePos;
            newPos.y += _currentBobY;
            currentVisual.transform.localPosition = newPos;
        }

        private void OnDestroy()
        {
            if (_playerMover != null)
                _playerMover.OnVelocityChanged -= OnVelocityChanged;
        }
    }
}
