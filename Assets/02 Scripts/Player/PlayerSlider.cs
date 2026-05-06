using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerSlider : MonoBehaviour, IModule
    {
        [Header("Slide (Base Values)")]
        [SerializeField] private float slideFriction = 7f;
        [SerializeField] private float slopeAcceleration = 8f;
        [SerializeField] private float minSlideDuration = 0.2f;
        [SerializeField] private float minSlideSpeed = 2f;
        [SerializeField] private LayerMask groundLayer;

        private float _slideBoostSpeed;
        private bool _isEnabled;

        [Header("Slide Jump")]
        [SerializeField] private float slideJumpUpForce = 6f;
        [SerializeField] private float slideJumpForwardMultiplier = 0.6f;

        [Header("Collider")]
        [SerializeField] private float crouchHeightRatio = 0.5f;

        private Player _player;
        private PlayerInputSO _playerInputSO;
        private PlayerMover _playerMover;
        private CapsuleCollider _collider;

        private float _standHeight;
        private float _standCenterY;
        private float _crouchHeight;

        private bool _isSliding;
        private bool _isCrouching;
        private Vector3 _slideDir;
        private float _slideSpeed;
        private float _slideStartTime;

        public bool IsSliding => _isSliding;
        public bool IsCrouching => _isCrouching;
        public float SlideSpeedRatio => _isSliding ? Mathf.Clamp01(_slideSpeed / _slideBoostSpeed) : 0f;

        public void Enable(float boostSpeed)
        {
            _isEnabled = true;
            _slideBoostSpeed = boostSpeed;
        }

        public void Disable()
        {
            _isEnabled = false;
            if (_isSliding) ForceEndSlide();
            if (_isCrouching)
            {
                _isCrouching = false;
                RestoreCollider();
            }
        }

        public void Initialize(ModuleOwner owner)
        {
            _player = owner as Player;
            _playerInputSO = _player.PlayerInputSO;
            _playerMover = _player.GetModule<PlayerMover>();
            _collider = GetComponent<CapsuleCollider>();
            if (_collider == null)
            {
                Debug.LogError("[PlayerSlider] Player에 CapsuleCollider가 없습니다. Inspector에서 추가해주세요.");
                return;
            }

            _standHeight = _collider.height;
            _standCenterY = _collider.center.y;
            _crouchHeight = _standHeight * crouchHeightRatio;

            _playerInputSO.OnSlideKeyPressed += OnCrouchPressed;
            _playerInputSO.OnJumpKeyPressed += OnJump;
        }

        private void OnDestroy()
        {
            if (_playerInputSO == null) return;
            _playerInputSO.OnSlideKeyPressed -= OnCrouchPressed;
            _playerInputSO.OnJumpKeyPressed -= OnJump;
        }

        private void OnCrouchPressed()
        {
            if (!_isEnabled) return;
            if (_isSliding) return;

            StartSlide();
        }

        private void OnJump()
        {
            if (_isCrouching)
            {
                TryEndCrouch();
                return;
            }

            if (!_isSliding) return;

            float savedSpeed = _slideSpeed;
            Vector3 savedDir = _slideDir;
            ForceEndSlide();

            _playerMover.StopImmediately(false, true, false);
            _playerMover.AddForceToAgent(savedDir * savedSpeed * slideJumpForwardMultiplier
                                       + Vector3.up * slideJumpUpForce);
        }

        private void StartSlide()
        {
            _isSliding = true;
            _slideStartTime = Time.time;

            Vector2 input = _playerInputSO.InputDirection;
            _slideDir = input.magnitude > 0.1f
                ? (_player.transform.forward * input.y + _player.transform.right * input.x).normalized
                : _player.transform.forward;

            _slideSpeed = _slideBoostSpeed;
            _playerMover.BeginSlide(_slideDir * _slideSpeed);

            ApplyCrouchCollider();
        }

        private void StartCrouch()
        {
            _isCrouching = true;
            ApplyCrouchCollider();
        }

        private void TryEndSlide()
        {
            if (!CanStandUp()) return;
            ForceEndSlide();
        }

        private void ForceEndSlide()
        {
            _isSliding = false;
            _playerMover.EndSlide();
            RestoreCollider();
        }

        private void TryEndCrouch()
        {
            if (!CanStandUp()) return;
            _isCrouching = false;
            RestoreCollider();
        }

        private void FixedUpdate()
        {
            if (_isSliding)
                UpdateSlide();
            else if (_isCrouching)
                UpdateCrouch();
        }

        private void UpdateSlide()
        {
            if (!_playerInputSO.IsSliding)
            {
                TryEndSlide();
                return;
            }

            if (!_playerMover.IsGrounded)
            {
                ForceEndSlide();
                return;
            }

            // 경사면 효과
            float slopeEffect = 0f;
            if (Physics.Raycast(transform.position + Vector3.up * 0.05f, Vector3.down, out RaycastHit hit, 1.2f, groundLayer))
            {
                Vector3 slopeFlat = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                slopeEffect = Vector3.Dot(_slideDir, slopeFlat);
            }

            float frictionThisFrame = slideFriction * Time.fixedDeltaTime;
            float slopeThisFrame = slopeEffect * slopeAcceleration * Time.fixedDeltaTime;
            _slideSpeed = Mathf.Max(0f, _slideSpeed - frictionThisFrame + slopeThisFrame);

            _playerMover.UpdateSlideVelocity(_slideDir * _slideSpeed);


        }

        private void UpdateCrouch()
        {
            // Slide 키를 놓으면 자동으로 일어섬
            if (!_playerInputSO.IsSliding)
                TryEndCrouch();
        }

        private void ApplyCrouchCollider()
        {
            float bottom = _standCenterY - _standHeight * 0.5f;
            _collider.height = _crouchHeight;
            _collider.center = new Vector3(_collider.center.x, bottom + _crouchHeight * 0.5f, _collider.center.z);
        }

        private void RestoreCollider()
        {
            _collider.height = _standHeight;
            _collider.center = new Vector3(_collider.center.x, _standCenterY, _collider.center.z);
        }

        private bool CanStandUp()
        {
            float radius = _collider.radius * 0.85f;
            Vector3 origin = transform.position + Vector3.up * (_crouchHeight * 0.5f + radius);
            float dist = _standHeight - _crouchHeight - radius;
            return !Physics.SphereCast(origin, radius, Vector3.up, out _, Mathf.Max(0f, dist));
        }
    }
}
