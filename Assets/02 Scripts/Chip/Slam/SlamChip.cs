using _02_Scripts.Agent;
using _02_Scripts.Core.Utility;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Chip.Slam
{
    [Chip("Slam")]
    public class SlamChip : IChip
    {
        private PlayerInputSO _playerInputSO;
        private PlayerMover _playerMover;
        private Player.Player _player;
        private SlamChipDataSO _data;

        private int _damage;
        private float _damageRadius;
        private bool _isSlamming;

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            _player = player;
            _playerInputSO = player.PlayerInputSO;
            _playerMover = player.GetModule<PlayerMover>();

            _playerInputSO.OnSlideKeyPressed -= TrySlam;
            _playerMover.OnGroundStatusChanged -= OnGrounded;

            _playerInputSO.OnSlideKeyPressed += TrySlam;
            _playerMover.OnGroundStatusChanged += OnGrounded;

            ApplyLevelStats(chip);
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            _playerInputSO.OnSlideKeyPressed -= TrySlam;
            _playerMover.OnGroundStatusChanged -= OnGrounded;
        }

        public void OnLevelUp(ChipInstance chip) => ApplyLevelStats(chip);

        private void ApplyLevelStats(ChipInstance chip)
        {
            if (chip.Data is SlamChipDataSO slamData)
            {
                _data = slamData;
                var levelData = slamData.LevelData[chip.CurrentLevel - 1];
                _damage = levelData.Damage;
                _damageRadius = levelData.DamageRadius;
            }
        }

        private void TrySlam()
        {
            if (_playerMover.IsGrounded) return;

            _isSlamming = true;
            _playerMover.SetVerticalVelocity(-_data.SlamDownSpeed);
        }

        private void OnGrounded(bool isGrounded)
        {
            if (!isGrounded || !_isSlamming) return;

            _isSlamming = false;
            SpawnDustEffect();
            ShakeCamera();
            DealAoeDamage();
        }

        private void SpawnDustEffect()
        {
            if (_data.DustEffectPrefab == null) return;
            var fx = Object.Instantiate(_data.DustEffectPrefab, _player.transform.position, Quaternion.identity);
            Object.Destroy(fx, 3f);
        }

        private void ShakeCamera()
        {
            
        }

        private void DealAoeDamage() //착지 대미지
        {
            var hits = Physics.OverlapSphere(_player.transform.position, _damageRadius, _data.EnemyLayer);
            foreach (var hit in hits)
            {
                var health = hit.GetComponent<AgentHealth>();
                if (health != null && hit.gameObject != _player.gameObject)
                    health.ApplyDamage(_damage);
            }
        }
    }
}
