using System;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Manager
{
    public class CursorManager  : MonoSingleton<CursorManager>
    {
        public void SetCursorVisible(bool visible)
        {
            Cursor.visible = visible;
            Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        }

    }
}