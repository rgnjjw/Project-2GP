using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/PlayerInputSO", order = 0)]
    public class PlayerInputSO : ScriptableObject, Controls.IPlayerActions
    {
        public Vector2 InputDirection { get; private set; }
        public Vector2 MouseDelta { get; private set; }
        public bool IsSliding { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsFireHeld { get; private set; }
        public bool IsSkillHeld { get; private set; }

        public event Action<int, InputAction.CallbackContext> OnChipInput;
        public event Action OnJumpKeyPressed;
        public event Action OnJumpKeyReleased;
        public event Action OnDashKeyPressed;
        public event Action OnSlideKeyPressed;
        public event Action OnRunKeyPressed;
        public event Action OnRunKeyReleased;
        public event Action OnFireKeyPressed;
        public event Action<float> OnScrollWeaponInput;
        public event Action OnWeapon1Pressed;
        public event Action OnWeapon2Pressed;
        public event Action OnWeapon3Pressed;
        public event Action OnSkillKeyPressed;
        public event Action OnSkillKeyReleased;

        // ScriptableObject는 씬을 다시 로드해도 살아있어, 이전 세션에서 등록된(이미 파괴된 객체의)
        // 이벤트 구독이 그대로 남는다. 그 죽은 구독이 호출되면 예외가 나며 이벤트 체인이 끊겨
        // 새 플레이어의 대쉬/슬라이드/공격 등이 동작하지 않는다. → 새 플레이어 생성 시 전부 비운다.
        public void ClearAllEvents()
        {
            OnChipInput = null;
            OnJumpKeyPressed = null;
            OnJumpKeyReleased = null;
            OnDashKeyPressed = null;
            OnSlideKeyPressed = null;
            OnRunKeyPressed = null;
            OnRunKeyReleased = null;
            OnFireKeyPressed = null;
            OnScrollWeaponInput = null;
            OnWeapon1Pressed = null;
            OnWeapon2Pressed = null;
            OnWeapon3Pressed = null;
            OnSkillKeyPressed = null;
            OnSkillKeyReleased = null;
        }

        public void OnScrollWeapon(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnScrollWeaponInput?.Invoke(context.ReadValue<Vector2>().y);
        }

        public void OnWeapon1(InputAction.CallbackContext context)
        {
            if (context.started)
                OnWeapon1Pressed?.Invoke();
        }

        public void OnWeapon2(InputAction.CallbackContext context)
        {
            if (context.started)
                OnWeapon2Pressed?.Invoke();
        }

        public void OnWeapon3(InputAction.CallbackContext context)
        {
            if (context.started)
                OnWeapon3Pressed?.Invoke();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            InputDirection = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            MouseDelta = context.ReadValue<Vector2>();
        }

        public void OnSlide(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsSliding = true;
                OnSlideKeyPressed?.Invoke();
            }
            else if (context.canceled)
            {
                IsSliding = false;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsJumping = true;
                OnJumpKeyPressed?.Invoke();
            }
            else if (context.canceled)
            {
                IsJumping = false;
                OnJumpKeyReleased?.Invoke();
            }
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.started)
                OnDashKeyPressed?.Invoke();
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsRunning = true;
                OnRunKeyPressed?.Invoke();
            }
            else if (context.canceled)
            {
                IsRunning = false;
                OnRunKeyReleased?.Invoke();
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsFireHeld = true;
                OnFireKeyPressed?.Invoke();
            }
            else if (context.canceled)
            {
                IsFireHeld = false;
            }
        }

        public void OnModuleAction(InputAction.CallbackContext context)
        {
            int index = context.action.GetBindingIndexForControl(context.control);
            OnChipInput?.Invoke(index, context);
        }

        public void OnSkill(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                IsSkillHeld = true;
                OnSkillKeyPressed?.Invoke();
            }
            else if (context.canceled)
            {
                IsSkillHeld = false;
                OnSkillKeyReleased?.Invoke();
            }
        }
    }
}
