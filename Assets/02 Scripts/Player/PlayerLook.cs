using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerLook : MonoBehaviour, IModule
    { 
        [SerializeField] private float mouseSensitivity;
        [SerializeField] public float clampAngleX = 45f;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private float cameraSmoothSpeed = 2f;
        
        private float _rotationX;
        private float _rotationY;
        private Player _player;
        private PlayerInputSO _playerInputSO;
        
        public void Initialize(ModuleOwner moduleOwner)
        {
            _player = moduleOwner as Player;
            
            if (_player != null) 
                _playerInputSO = _player.PlayerInputSO;
        }

        private void Look()
        {
            float angleX = _playerInputSO.MouseDelta.x * mouseSensitivity;
            float angleY = _playerInputSO.MouseDelta.y * mouseSensitivity;
            
            _rotationX -= angleY;
            _rotationY += angleX;
            
            _rotationX = Mathf.Clamp(_rotationX, -clampAngleX, clampAngleX);
            
            Quaternion camTargetRotation = Quaternion.Euler(_rotationX,0, 0);
            Quaternion playerTargetRotation = Quaternion.Euler(0,_rotationY, 0);

            playerCamera.transform.localRotation = camTargetRotation;
            _player.transform.rotation = playerTargetRotation;
        }

        public void Update()
        {
            Look();
        }
    }
}