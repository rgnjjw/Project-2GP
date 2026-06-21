using csiimnida.CSILib.SoundManager.RunTime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _02_Scripts.UI
{
    public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverScale = 1.2f;
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease ease = Ease.OutBack;

        private Vector3 _originalScale;
        private Tween _tween;

        private void Awake()
        {
            _originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = transform.DOScale(_originalScale * hoverScale, duration).SetEase(ease).SetUpdate(true);
            SoundManager.Instance.PlaySound("UIHover");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _tween?.Kill();
            _tween = transform.DOScale(_originalScale, duration).SetEase(ease).SetUpdate(true);
        }

        private void OnDisable()
        {
            _tween?.Kill();
            transform.localScale = _originalScale;
        }
    }
}
