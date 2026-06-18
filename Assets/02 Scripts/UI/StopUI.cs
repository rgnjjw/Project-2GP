using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.UI
{
    public class StopUI : MonoBehaviour
    {
        [SerializeField] private GameObject stopUI;
        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                stopUI.SetActive(true);
            }
        }
    }
}