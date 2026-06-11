using System.Collections;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Chip.Run
{
    [Chip("Run")]
    public class RunChip : IChip, IStaminaProvider
    {
        private PlayerInputSO _playerInputSO;
        private PlayerMover _playerMover;
        private Player.Player _player;

        private float _maxStamina;
        private float _runSpeedMultiplier;
        private float _rechargeDelay;
        private float _rechargeRate;

        private float _stamina;
        private bool _isRunning;
        private float _stopTime = -999f;
        private Coroutine _updateRoutine;

        public float Stamina => _stamina;
        public float MaxStamina => _maxStamina;

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            _player = player;
            _playerInputSO = player.PlayerInputSO;
            _playerMover = player.GetModule<PlayerMover>();

            ApplyLevelStats(chip);
            _stamina = _maxStamina;

            if (_updateRoutine != null) _player.StopCoroutine(_updateRoutine);
            _updateRoutine = _player.StartCoroutine(RunUpdateRoutine());
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            StopRun();
            if (_updateRoutine != null)
            {
                _player.StopCoroutine(_updateRoutine);
                _updateRoutine = null;
            }
        }

        public void OnLevelUp(ChipInstance chip) => ApplyLevelStats(chip);

        private void ApplyLevelStats(ChipInstance chip)
        {
            if (chip.Data is RunChipDataSO data)
            {
                var level = data.LevelData[chip.CurrentLevel - 1];
                _maxStamina = level.StaminaDuration;
                _runSpeedMultiplier = level.RunSpeedMultiplier;
                _rechargeDelay = level.RechargeDelay;
                _rechargeRate = level.RechargeRate;
                _stamina = Mathf.Min(_stamina, _maxStamina);
            }
        }

        private IEnumerator RunUpdateRoutine()
        {
            while (true)
            {
                bool wantsToRun = _playerInputSO.IsRunning
                                  && _playerMover.IsGrounded
                                  && _playerInputSO.InputDirection.magnitude > 0.1f;

                if (wantsToRun && _stamina > 0f)
                {
                    if (!_isRunning) StartRun();
                    _stamina = Mathf.Max(0f, _stamina - Time.deltaTime);
                    if (_stamina <= 0f) StopRun();
                }
                else if (_isRunning)
                {
                    StopRun();
                }
                else if (Time.time - _stopTime >= _rechargeDelay && _stamina < _maxStamina)
                {
                    _stamina = Mathf.Min(_stamina + _rechargeRate * Time.deltaTime, _maxStamina);
                }

                yield return null;
            }
        }

        private void StartRun()
        {
            _isRunning = true;
            _playerMover.SetMoveSpeedMultiplier(_runSpeedMultiplier);
        }

        private void StopRun()
        {
            if (!_isRunning) return;
            _isRunning = false;
            _stopTime = Time.time;
            _playerMover.SetMoveSpeedMultiplier(1f);
        }
    }
}
