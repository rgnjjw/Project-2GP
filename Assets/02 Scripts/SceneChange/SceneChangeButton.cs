using UnityEngine;
using UnityEngine.UI;
using _02_Scripts.Manager;
using _02_Scripts.UI;

namespace _02_Scripts.SceneChange
{
    [RequireComponent(typeof(Button))]
    public class SceneChangeButton : MonoBehaviour
    {
        [SerializeField] private SceneReference targetScene;
        [SerializeField] private bool useSingleLoad = false;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClickChangeScene);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClickChangeScene);
        }

        private void OnClickChangeScene()
        {
            string sceneName = targetScene.SceneName;

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning($"[SceneChangeButton] sceneName이 비어있음: {gameObject.name}");
                return;
            }

            _button.interactable = false;

            var pauseManager = FindFirstObjectByType<PauseManager>();
            if (pauseManager != null)
            {
                pauseManager.RunAfterSubPanelClosed(() => LoadSceneAsync(sceneName));
            }
            else
            {
                LoadSceneAsync(sceneName);
            }
        }

        private async void LoadSceneAsync(string sceneName)
        {
            Time.timeScale = 1f;

            if (useSingleLoad)
            {
                await GameSceneManager.Instance.LoadOneSceneAsync(sceneName);
            }
            else
            {
                await GameSceneManager.Instance.LoadSceneAsync(sceneName);
            }
        }
    }
}