using _02_Scripts.Core.Utility;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Chip.Jump
{
    [Chip("Jump")]
    public class JumpChip : IChip
    {
        private PlayerInputSO _playerInputSO;
        private PlayerMover _playerMover;
        private float _jumpPower;
        private int _maxJumpCount;
        private int _currentJumpCount;

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            _playerInputSO = player.PlayerInputSO;
            _playerMover = player.GetModule<PlayerMover>();

            _playerInputSO.OnJumpKeyPressed -= Jump;
            _playerMover.OnGroundStatusChanged -= JumpCountReset;

            _playerInputSO.OnJumpKeyPressed += Jump;
            _playerMover.OnGroundStatusChanged += JumpCountReset;

            ApplyLevelStats(chip);
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            _playerInputSO.OnJumpKeyPressed -= Jump;
            _playerMover.OnGroundStatusChanged -= JumpCountReset;
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
            if (_currentJumpCount >= _maxJumpCount) return;
            EventBus.Publish(new EffectEvent("JumpDust"));
            _playerMover.AddForceToAgent(Vector3.up * _jumpPower);
            _currentJumpCount++;
        }

        private void JumpCountReset(bool isGrounded)
        {
            if (isGrounded)
                _currentJumpCount = 0;
        }
    }
}
