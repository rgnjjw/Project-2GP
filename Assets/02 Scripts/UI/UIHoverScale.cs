using csiimnida.CSILib.SoundManager.RunTime;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    public class UIHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverScale = 1.2f;
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private Ease ease = Ease.OutBack;

        [Header("Optional Hover Image")]
        [SerializeField] private Image hoverImage;

        private Vector3 _originalScale;
        private Vector3 _originalImageScale;
        private Tween _scaleTween;
        private Tween _imageScaleTween;

        private void Awake()
        {
            _originalScale = transform.localScale;

            if (hoverImage != null)
            {
                _originalImageScale = hoverImage.transform.localScale;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(_originalScale * hoverScale, duration).SetEase(ease).SetUpdate(true);
            SoundManager.Instance.PlaySound("UIHover");

            if (hoverImage != null)
            {
                _imageScaleTween?.Kill();
                _imageScaleTween = hoverImage.transform.DOScale(_originalImageScale * hoverScale, duration).SetEase(ease).SetUpdate(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _scaleTween?.Kill();
            _scaleTween = transform.DOScale(_originalScale, duration).SetEase(ease).SetUpdate(true);

            if (hoverImage != null)
            {
                _imageScaleTween?.Kill();
                _imageScaleTween = hoverImage.transform.DOScale(_originalImageScale, duration).SetEase(ease).SetUpdate(true);
            }
        }

        private void OnDisable()
        {
            _scaleTween?.Kill();
            _imageScaleTween?.Kill();
            transform.localScale = _originalScale;

            if (hoverImage != null)
            {
                hoverImage.transform.localScale = _originalImageScale;
            }
        }
    }
}