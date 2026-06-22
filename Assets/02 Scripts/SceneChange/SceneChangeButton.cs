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

            // GameSceneManager(부트 매니저)가 없으면(예: 에디터에서 MainScene 직접 Play)
            // 직접 단일 로드로 폴백한다 → Instance null로 인한 NullReference 방지.
            if (GameSceneManager.Instance == null)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
                return;
            }

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