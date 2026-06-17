using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _02_Scripts.UI.StyleBar
{
    public class StyleFeedItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            if (text == null)
            {
                text = GetComponentInChildren<TMP_Text>();
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }

        public void Setup(string message, float displayTime)
        {
            text.text = message;
            canvasGroup.alpha = 1f;

            DOVirtual.DelayedCall(displayTime, () =>
            {
                canvasGroup.DOFade(0f, 0.4f).OnComplete(() => Destroy(gameObject));
            });
        }
    }
}
