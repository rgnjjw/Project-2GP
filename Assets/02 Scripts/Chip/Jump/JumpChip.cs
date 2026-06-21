using _02_Scripts.Player;
using csiimnida.CSILib.SoundManager.RunTime;
using UnityEngine;

namespace _02_Scripts.Chip.Jump
{
    [Chip("Jump")]
    public class JumpChip : IChip
    {
        private Player.Player _player;
        private PlayerInputSO _playerInputSO;
        private PlayerMover _playerMover;
        
        private float _jumpPower;
        private int _maxJumpCount;
        private int _currentJumpCount;

        private const float JumpBufferWindow = 0.1f;
        private float _jumpBufferTime = -999f;

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            _player = player;
            _playerInputSO = player.PlayerInputSO;
            _playerMover = player.GetModule<PlayerMover>();

            _playerInputSO.OnJumpKeyPressed -= Jump;
            _playerMover.OnGroundStatusChanged -= OnGroundStatusChanged;
            _player.OnWallRideEnded -= OnWallRideEnded;

            _playerInputSO.OnJumpKeyPressed += Jump;
            _playerMover.OnGroundStatusChanged += OnGroundStatusChanged;
            _player.OnWallRideEnded += OnWallRideEnded;

            ApplyLevelStats(chip);
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            _playerInputSO.OnJumpKeyPressed -= Jump;
            _playerMover.OnGroundStatusChanged -= OnGroundStatusChanged;
            _player.OnWallRideEnded -= OnWallRideEnded;
        }

        public void OnLevelUp(ChipInstance chip) => ApplyLevelStats(chip);

        private void ApplyLevelStats(ChipInstance chip)
        {
            if (chip.Data is JumpChipDataSO jumpData)
            {
                var levelData = jumpData.LevelData[chip.CurrentLevel - 1];
                _jumpPower = levelData.JumpPower;
                _maxJumpCount = levelData.MaxJumpCount;
            }
        }

        private void Jump()
        {
            if (_player.IsWallRiding) return;
            if (_currentJumpCount >= _maxJumpCount)
            {
                _jumpBufferTime = Time.time;
                return;
            }
            ExecuteJump();
        }

        private void OnGroundStatusChanged(bool isGrounded)
        {
            if (!isGrounded) return;
            _currentJumpCount = 0;

            if (Time.time - _jumpBufferTime <= JumpBufferWindow)
            {
                _jumpBufferTime = -999f;
                ExecuteJump();
            }
        }

        private void OnWallRideEnded()
        {
            _currentJumpCount = 0;
            if (_playerInputSO.IsJumping)
                ExecuteJump();
        }

        private void ExecuteJump()
        {
            _playerMover.StopImmediately(false, true, false);
            _playerMover.AddForceToAgent(Vector3.up * _jumpPower);
            SoundManager.Instance.PlaySound("Jump");
            _currentJumpCount++;
        }
    }
}
