using _02_Scripts.Core.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.UI
{
    public class PauseManager : MonoSingleton<PauseManager>
    {
        [SerializeField] private GameObject pauseMenuPanel;

        public bool IsPaused { get; private set; }
        public bool IsSubPanelOpen { get; private set; }

        private void Awake()
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
            pauseMenuPanel.SetActive(true);
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;
            pauseMenuPanel.SetActive(false);
        }

        public void OpenSubPanel()
        {
            IsSubPanelOpen = true;
        }

        public void CloseSubPanel()
        {
            IsSubPanelOpen = false;
        }
    }
}