using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.Player
{
    [CreateAssetMenu(fileName = "PlayerInputSO", menuName = "Player/PlayerInputSO", order = 0)]
    public class PlayerInputSO : ScriptableObject,Controls.IPlayerActions
    {
        public Vector2 InputDirection { get;private set; }
        public Vector2 MouseDelta { get;private set; }  
        public bool IsSliding { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsJumping { get; private set; }
        public event Action<int,InputAction.CallbackContext> OnChipInput;
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
            if(context.started)
                OnFireKeyPressed?.Invoke();
        }

        public void OnModuleAction(InputAction.CallbackContext context)
        {
            int index = context.action.GetBindingIndexForControl(context.control);
            OnChipInput?.Invoke(index, context);
        }
    }
}