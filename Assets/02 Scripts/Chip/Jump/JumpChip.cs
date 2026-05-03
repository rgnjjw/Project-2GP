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
            
            //방어코드
            _playerInputSO.OnJumpKeyPressed -= Jump;
            _playerMover.OnGroundStatusChanged -= JumpCountReset;
            
            _playerInputSO.OnJumpKeyPressed += Jump;
            _playerMover.OnGroundStatusChanged += JumpCountReset;
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            _playerInputSO.OnJumpKeyPressed -= Jump;
            _playerMover.OnGroundStatusChanged -= JumpCountReset;
        }

        public void OnLevelUp(ChipInstance chip)
        {
            if (chip.Data is JumpChipDataSO jumpChipData)
            {
                _jumpPower = jumpChipData.LevelData[chip.CurrentLevel].JumpPower;
                _maxJumpCount = jumpChipData.LevelData[chip.CurrentLevel].MaxJumpCount;
            }
        }

        private void Jump()
        {
            if (_currentJumpCount >= _maxJumpCount) return;
            _playerMover.AddForceToAgent(Vector3.up *  _jumpPower);
            _currentJumpCount++;
        }
        
        private void JumpCountReset(bool isGrounded)
        {
            if(isGrounded)
                _currentJumpCount = 0;
        }
    }
}