using System.Collections;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Chip.Dash
{
    [Chip("Dash")]
    public class DashChip : IChip
    {
        private PlayerInputSO _playerInputSO;
        private PlayerMover _playerMover;
        private Player.Player _player;

        private float _dashSpeed;
        private float _dashDuration;
        private float _rechargeTime;
        private int _maxDashCount;
        private int _currentDashCount;
        private Coroutine _rechargeCoroutine;

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            _playerInputSO = player.PlayerInputSO;
            _playerMover = player.GetModule<PlayerMover>();
            _player = player;

            _playerInputSO.OnDashKeyPressed -= Dash;
            _playerInputSO.OnDashKeyPressed += Dash;

            ApplyLevelStats(chip);
            _currentDashCount = _maxDashCount;
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            _playerInputSO.OnDashKeyPressed -= Dash;
            StopRecharge();
        }

        public void OnLevelUp(ChipInstance chip)
        {
            ApplyLevelStats(chip);
            if (_rechargeCoroutine != null)
            {
                StopRecharge();
                _rechargeCoroutine = _player.StartCoroutine(RechargeRoutine());
            }
        }

        private void ApplyLevelStats(ChipInstance chip)
        {
            if (chip.Data is DashChipDataSO dashData)
            {
                var levelData = dashData.LevelData[chip.CurrentLevel - 1];
                _dashSpeed = levelData.DashSpeed;
                _dashDuration = levelData.DashDuration;
                _rechargeTime = levelData.RechargeTime;
                _maxDashCount = levelData.MaxDashCount;
                _currentDashCount = Mathf.Min(_currentDashCount, _maxDashCount);
            }
        }

        private void Dash()
        {
            if (_currentDashCount <= 0) return;

            Vector2 input = _playerInputSO.InputDirection;
            Vector3 dashDir = input.magnitude > 0.1f
                ? (_player.transform.forward * input.y + _player.transform.right * input.x).normalized
                : _player.transform.forward;

            _playerMover.SetDashVelocity(dashDir * _dashSpeed, _dashDuration);

            _currentDashCount--;

            if (_rechargeCoroutine == null)
                _rechargeCoroutine = _player.StartCoroutine(RechargeRoutine());
        }

        private IEnumerator RechargeRoutine()
        {
            while (_currentDashCount < _maxDashCount)
            {
                yield return new WaitForSeconds(_rechargeTime);
                _currentDashCount = Mathf.Min(_currentDashCount + 1, _maxDashCount);
            }
            _rechargeCoroutine = null;
        }

        private void StopRecharge()
        {
            if (_rechargeCoroutine != null)
            {
                _player.StopCoroutine(_rechargeCoroutine);
                _rechargeCoroutine = null;
            }
        }
    }
}
