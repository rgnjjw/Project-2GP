using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerLook : MonoBehaviour, IModule, IAfterInitModule
    {
        [SerializeField] private float mouseSensitivity;
        [SerializeField] public float clampAngleX = 45f;
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private float cameraSmoothSpeed = 2f;

        [Header("Slide Camera")]
        [SerializeField] private float crouchHeightRatio = 0.55f;
        [SerializeField] private float cameraHeightLerpSpeed = 14f;
        [SerializeField] private float slideTiltAngle = 6f;
        [SerializeField] private float tiltLerpSpeed = 10f;
        [SerializeField] private float maxFOVBoost = 7f;
        [SerializeField] private float fovLerpSpeed = 8f;

        private float _rotationX;
        private float _rotationY;
        private Player _player;
        private PlayerInputSO _playerInputSO;
        private PlayerSlider _playerSlider;

        private Camera _camera;
        private float _defaultFOV;
        private float _standingCamHeight;
        private float _currentCamHeight;
        private float _currentTilt;
        private float _currentFOVBoost;

        private void Awake()
        {
            if (playerCamera != null)
            {
                _standingCamHeight = playerCamera.transform.localPosition.y;
                _currentCamHeight = _standingCamHeight;
                _camera = playerCamera.GetComponent<Camera>();
                if (_camera != null) _defaultFOV = _camera.fieldOfView;
            }
        }

        public void Initialize(ModuleOwner moduleOwner)
        {
            _player = moduleOwner as Player;
            if (_player != null)
                _playerInputSO = _player.PlayerInputSO;
        }

        public void AfterInit()
        {
            _playerSlider = _player != null ? _player.GetModule<PlayerSlider>() : null;
        }

        private void Look()
        {
            float angleX = _playerInputSO.MouseDelta.x * mouseSensitivity;
            float angleY = _playerInputSO.MouseDelta.y * mouseSensitivity;

            _rotationX -= angleY;
            _rotationY += angleX;

            _rotationX = Mathf.Clamp(_rotationX, -clampAngleX, clampAngleX);

            _currentTilt = Mathf.Lerp(_currentTilt, GetTargetTilt(), tiltLerpSpeed * Time.deltaTime);

            Quaternion camTargetRotation = Quaternion.Euler(_rotationX + _currentTilt, 0, 0);
            Quaternion playerTargetRotation = Quaternion.Euler(0, _rotationY, 0);

            playerCamera.transform.localRotation = camTargetRotation;
            _player.transform.rotation = playerTargetRotation;
        }

        private void UpdateSlideEffects()
        {
            float targetHeight = GetTargetCameraHeight();
            _currentCamHeight = Mathf.Lerp(_currentCamHeight, targetHeight, cameraHeightLerpSpeed * Time.deltaTime);

            Vector3 camPos = playerCamera.transform.localPosition;
            camPos.y = _currentCamHeight;
            playerCamera.transform.localPosition = camPos;

            if (_camera != null)
            {
                float targetFOV = _defaultFOV + GetTargetFOVBoost();
                _currentFOVBoost = Mathf.Lerp(_currentFOVBoost, targetFOV, fovLerpSpeed * Time.deltaTime);
                _camera.fieldOfView = _currentFOVBoost;
            }
        }

        private float GetTargetCameraHeight()
        {
            if (_playerSlider != null && (_playerSlider.IsSliding || _playerSlider.IsCrouching))
                return _standingCamHeight * crouchHeightRatio;
            return _standingCamHeight;
        }

        private float GetTargetTilt()
        {
            if (_playerSlider != null && _playerSlider.IsSliding)
                return slideTiltAngle;
            return 0f;
        }

        private float GetTargetFOVBoost()
        {
            if (_playerSlider != null && _playerSlider.IsSliding)
                return maxFOVBoost * _playerSlider.SlideSpeedRatio;
            return 0f;
        }

        public void Update()
        {
            Look();
            UpdateSlideEffects();
        }
    }
}
