using System.Threading.Tasks;
using _02_Scripts.Manager;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.SceneChange
{
    public class LoadingScreenController : MonoBehaviour
    {
        public static LoadingScreenController Instance { get; private set; }

        [SerializeField] private RectTransform panel;
        [SerializeField] private CanvasGroup logoGroup;
        [SerializeField] private float slideDuration = 0.4f;
        [SerializeField] private float minimumShowTime = 1.5f;

        private float _showStartTime;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            panel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (GameSceneManager.Instance != null)
            {
                GameSceneManager.Instance.OnBeforeLoadAsync = ShowAsync;
                GameSceneManager.Instance.OnAfterLoadAsync = HideAsync;
            }
        }

        public async Task ShowAsync()
        {
            panel.gameObject.SetActive(true);
            panel.anchoredPosition = new Vector2(Screen.width, 0);
            logoGroup.alpha = 0f;

            var tcs = new TaskCompletionSource<bool>();

            panel.DOAnchorPos(Vector2.zero, slideDuration)
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    logoGroup.DOFade(1f, 0.2f).SetUpdate(true);
                    tcs.SetResult(true);
                });

            _showStartTime = Time.unscaledTime;
            await tcs.Task;
        }

        public async Task HideAsync()
        {
            float elapsed = Time.unscaledTime - _showStartTime;
            float remaining = minimumShowTime - elapsed;
            if (remaining > 0f)
            {
                await Task.Delay((int)(remaining * 1000));
            }

            var tcs = new TaskCompletionSource<bool>();

            logoGroup.DOFade(0f, 0.2f).SetUpdate(true);
            panel.DOAnchorPos(new Vector2(-Screen.width, 0), slideDuration)
                .SetEase(Ease.InCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    panel.gameObject.SetActive(false);
                    tcs.SetResult(true);
                });

            await tcs.Task;
        }
    }
}