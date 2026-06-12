using System.Collections;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Chip.WallRide
{
    [Chip("WallRide")]
    public class WallRideChip : IChip
    {
        private PlayerInputSO _input;
        private PlayerMover _mover;
        private PlayerLook _playerLook;
        private Player.Player _player;

        private float _wallRideSpeed;
        private float _wallJumpSideForce;
        private float _wallJumpUpForce;
        private LayerMask _wallLayer;

        private bool _isWallRiding;
        private Vector3 _wallNormal;
        private Vector3 _wallTangent;
        private Coroutine _updateRoutine;
        private int _wallLostFrames;

        private const float WallDetectDist = 1.0f;
        private const float WallTangentDetectDist = 1.5f; // 이동 방향은 더 멀리 감지
        private const float WallSinkSpeed = -0.5f;
        private const float DutchAngleDeg = 12f;
        private const float DutchTweenDur = 0.2f;
        private const float CheckHeightOffset = 0.5f;
        private const float SameWallDot = 0.7f;
        private const float WallExitForce = 20f;
        private const int WallLostGrace = 4; // 코너에서 벽 감지 끊겨도 4프레임 유예

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            _player = player;
            _input = player.PlayerInputSO;
            _mover = player.GetModule<PlayerMover>();
            _playerLook = player.GetModule<PlayerLook>();

            _input.OnJumpKeyPressed += OnJumpPressed;
            _input.OnJumpKeyReleased += OnJumpReleased;
            ApplyLevelStats(chip);

            if (_updateRoutine != null) _player.StopCoroutine(_updateRoutine);
            _updateRoutine = _player.StartCoroutine(UpdateRoutine());
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            _input.OnJumpKeyPressed -= OnJumpPressed;
            _input.OnJumpKeyReleased -= OnJumpReleased;

            if (_updateRoutine != null)
            {
                _player.StopCoroutine(_updateRoutine);
                _updateRoutine = null;
            }

            if (_isWallRiding) EndWallRide(false);
        }

        public void OnLevelUp(ChipInstance chip) => ApplyLevelStats(chip);

        private void ApplyLevelStats(ChipInstance chip)
        {
            if (chip.Data is not WallRideChipDataSO data) return;
            _wallLayer = data.WallLayer.value == 0 ? ~0 : data.WallLayer;
            var lv = data.LevelData[chip.CurrentLevel - 1];
            _wallRideSpeed = lv.WallRideSpeed;
            _wallJumpSideForce = lv.WallJumpSideForce;
            _wallJumpUpForce = lv.WallJumpUpForce;
        }

        private IEnumerator UpdateRoutine()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                if (_isWallRiding)
                {
                    if (TryDetectWallContinue(out Vector3 newNormal))
                    {
                        _wallLostFrames = 0;
                        UpdateWallNormal(newNormal);
                        ApplyWallRideMovement();
                    }
                    else
                    {
                        _wallLostFrames++;
                        if (_wallLostFrames >= WallLostGrace)
                        {
                            _wallLostFrames = 0;
                            EndWallRide(false, notifyJumpChip: true); // 벽 끝 자동 종료: JumpChip에 알림
                        }
                        // 유예 중: 마지막 방향으로 계속 이동하면서 새 벽 대기
                    }
                }
                else
                {
                    if (CanStartWallRide(out Vector3 wallNormal))
                        StartWallRide(wallNormal);
                }
            }
        }

        private bool CanStartWallRide(out Vector3 wallNormal)
        {
            wallNormal = Vector3.zero;
            return _input.IsJumping
                   && _input.InputDirection.magnitude > 0.1f
                   && TryDetectWall(out wallNormal);
        }

        private bool TryDetectWall(out Vector3 wallNormal)
        {
            wallNormal = Vector3.zero;
            Vector3 origin = _player.transform.position + Vector3.up * CheckHeightOffset;

            Vector3[] dirs = { _player.transform.right, -_player.transform.right, _player.transform.forward };
            foreach (Vector3 dir in dirs)
            {
                if (Physics.Raycast(origin, dir, out RaycastHit hit, WallDetectDist, _wallLayer)
                    && Mathf.Abs(hit.normal.y) < 0.3f)
                {
                    wallNormal = hit.normal;
                    return true;
                }
            }
            return false;
        }

        private bool TryDetectWallContinue(out Vector3 wallNormal)
        {
            wallNormal = Vector3.zero;
            Vector3 origin = _player.transform.position + Vector3.up * CheckHeightOffset;

            Vector3 sameWallHit = Vector3.zero;
            Vector3 newFaceHit = Vector3.zero;

            // _wallTangent(이동 방향)를 더 긴 거리로 먼저 체크
            if (Physics.Raycast(origin, _wallTangent, out RaycastHit tangentHit, WallTangentDetectDist, _wallLayer)
                && Mathf.Abs(tangentHit.normal.y) < 0.3f)
            {
                if (Vector3.Dot(tangentHit.normal, _wallNormal) > SameWallDot)
                    sameWallHit = tangentHit.normal;
                else
                    newFaceHit = tangentHit.normal;
            }

            // 나머지 방향 체크
            Vector3[] dirs =
            {
                -_wallNormal,
                _player.transform.right,
                -_player.transform.right,
                _player.transform.forward,
                -_player.transform.forward
            };

            foreach (Vector3 dir in dirs)
            {
                if (!Physics.Raycast(origin, dir, out RaycastHit hit, WallDetectDist, _wallLayer)) continue;
                if (Mathf.Abs(hit.normal.y) >= 0.3f) continue;

                if (Vector3.Dot(hit.normal, _wallNormal) > SameWallDot)
                {
                    if (sameWallHit == Vector3.zero) sameWallHit = hit.normal;
                }
                else
                {
                    if (newFaceHit == Vector3.zero) newFaceHit = hit.normal;
                }
            }

            if (newFaceHit != Vector3.zero) { wallNormal = newFaceHit; return true; }
            if (sameWallHit != Vector3.zero) { wallNormal = sameWallHit; return true; }
            return false;
        }

        private void StartWallRide(Vector3 wallNormal)
        {
            _isWallRiding = true;
            _player.IsWallRiding = true;
            _wallLostFrames = 0;
            _wallNormal = wallNormal;
            _wallTangent = ComputeTangent(wallNormal, Vector3.zero, _player.transform.forward);
            ApplyWallRideMovement();

            float side = Vector3.Dot(wallNormal, _player.transform.right) > 0f ? 1f : -1f;
            _playerLook.TweenDutchAngle(side * DutchAngleDeg, DutchTweenDur);
        }

        private void UpdateWallNormal(Vector3 newNormal)
        {
            if (Vector3.Dot(newNormal, _wallNormal) >= SameWallDot) return;

            Vector3 oldNormal = _wallNormal;
            _wallNormal = newNormal;
            _wallTangent = ComputeTangent(newNormal, oldNormal, _wallTangent);

            float side = Vector3.Dot(_wallNormal, _player.transform.right) > 0f ? 1f : -1f;
            _playerLook.TweenDutchAngle(side * DutchAngleDeg, DutchTweenDur);
        }

        private Vector3 ComputeTangent(Vector3 wallNormal, Vector3 oldNormal, Vector3 reference)
        {
            Vector3 tangent = Vector3.Cross(wallNormal, Vector3.up).normalized;
            float dot = Vector3.Dot(tangent, reference);

            if (!Mathf.Approximately(dot, 0f))
            {
                if (dot < 0f) tangent = -tangent;
                return tangent;
            }

            // 정확히 90도 코너: 이전 노멀 → 새 노멀의 회전 방향으로 결정
            if (oldNormal != Vector3.zero)
            {
                if (Vector3.Cross(oldNormal, wallNormal).y < 0f)
                    tangent = -tangent;
            }
            return tangent;
        }

        private void ApplyWallRideMovement()
        {
            _mover.BeginSlide(_wallTangent * _wallRideSpeed);
            _mover.SetVerticalVelocity(WallSinkSpeed);
        }

        // notifyJumpChip: 자동 종료 시 true → JumpChip count 리셋 + 스페이스 누르고 있으면 즉시 점프
        // 벽점프(OnJumpPressed)는 false로 호출 → JumpChip 개입 없음
        private void EndWallRide(bool applyExitForce, bool notifyJumpChip = false)
        {
            _isWallRiding = false;
            _player.IsWallRiding = false;
            _wallLostFrames = 0;
            _mover.EndSlide();
            _playerLook.TweenDutchAngle(0f, DutchTweenDur);

            if (applyExitForce)
            {
                Vector3 launchDir = _player.transform.forward;
                launchDir.y = 0f;
                if (launchDir.sqrMagnitude > 0.001f)
                    _mover.ForceSetXZVelocity(launchDir.normalized * WallExitForce);
            }

            if (notifyJumpChip)
                _player.FireWallRideEnded();
        }

        private void OnJumpReleased()
        {
            if (!_isWallRiding) return;
            EndWallRide(true, notifyJumpChip: true);
        }

        private void OnJumpPressed()
        {
            if (!_isWallRiding) return;

            Vector3 kickNormal = _wallNormal;
            EndWallRide(false, notifyJumpChip: false); // 벽점프: JumpChip 개입 없음

            _mover.StopImmediately(true, true, true);
            _mover.AddForceToAgent(kickNormal * _wallJumpSideForce + Vector3.up * _wallJumpUpForce);
            _mover.SyncMomentumFromVelocity();
        }
    }
}
