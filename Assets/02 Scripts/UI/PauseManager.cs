using System;
using System.Collections;
using _02_Scripts.Core.Utility;
using _02_Scripts.Manager;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.UI
{
    public class PauseManager : MonoSingleton<PauseManager>
    {
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private SlidePanel soundPanel;

        public bool IsPaused { get; private set; }
        public bool IsSubPanelOpen => soundPanel != null && soundPanel.IsOpen;
        public bool JustResumed { get; private set; }

        protected override void Awake()
        {
            pauseMenuPanel.SetActive(false);
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                HandleEscapePressed();
            }
        }

        private void HandleEscapePressed()
        {
            if (IsSubPanelOpen)
            {
                soundPanel.Close();
                return;
            }

            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0f;
            CursorManager.Instance.SetCursorVisible(true);
            pauseMenuPanel.SetActive(true);
        }

        public void Resume()
        {
            RunAfterSubPanelClosed(ResumeInternal);
        }

        private void ResumeInternal()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            pauseMenuPanel.SetActive(false);
            StartCoroutine(ResumeRoutine());
        }

        private IEnumerator ResumeRoutine()
        {
            JustResumed = true;
            yield return null;
            JustResumed = false;
            CursorManager.Instance.SetCursorVisible(false);
        }

        public void RunAfterSubPanelClosed(Action action)
        {
            if (IsSubPanelOpen)
            {
                soundPanel.Close(onComplete: action);
            }
            else
            {
                action?.Invoke();
            }
        }

        public void ToggleSubPanel()
        {
            soundPanel.Toggle();
        }
    }
}