using UnityEngine;
using UnityEngine.UI;
using _02_Scripts.Manager;

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

        private async void OnClickChangeScene()
        {
            string sceneName = targetScene.SceneName;

            if (string.IsNullOrEmpty(sceneName))
            {
                Debug.LogWarning($"[SceneChangeButton] sceneName이 비어있음: {gameObject.name}");
                return;
            }

            _button.interactable = false;

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