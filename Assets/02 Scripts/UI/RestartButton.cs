using UnityEngine;
using UnityEngine.SceneManagement;

namespace _02_Scripts.UI
{
    public class RestartButton : MonoBehaviour
    {
        public void OnClickRestart()
        {
            PauseManager.Instance.RunAfterSubPanelClosed(RestartCurrentScene);
        }

        private void RestartCurrentScene()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}