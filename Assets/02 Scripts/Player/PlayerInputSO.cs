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
        public bool IsSliding { get;private set; }
        public event Action<int,InputAction.CallbackContext> OnChipInput;
        public event Action OnJumpKeyPressed;
        public event Action OnDashKeyPressed;
        public event Action OnSlideKeyPressed;

        public void OnMove(InputAction.CallbackContext context)
        {
            InputDirection = context.ReadValue<Vector2>();
            Debug.Log(InputDirection);
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
                OnJumpKeyPressed?.Invoke();
        }

        public void OnDash(InputAction.CallbackContext context)
        {
            if (context.started)
                OnDashKeyPressed?.Invoke();
        }

        public void OnModuleAction(InputAction.CallbackContext context)
        {
            int index = context.action.GetBindingIndexForControl(context.control);
            OnChipInput?.Invoke(index, context);
        }
    }
}