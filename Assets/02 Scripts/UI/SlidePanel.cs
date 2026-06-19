using System;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.UI
{
    public class SlidePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private Vector2 hiddenPos;
        [SerializeField] private Vector2 shownPos;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutCubic;

        private bool _isOpen;
        private bool _isAnimating;
        private Tween _currentTween;

        public bool IsOpen => _isOpen;

        public void Toggle()
        {
            if (_isOpen) Close();
            else Open();
        }

        public void Open(Action onComplete = null)
        {
            if (_isAnimating || _isOpen)
            {
                onComplete?.Invoke();
                return;
            }

            _isOpen = true;
            _isAnimating = true;

            _currentTween?.Kill();
            _currentTween = panelRect.DOAnchorPos(shownPos, slideDuration)
                .SetEase(slideEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _isAnimating = false;
                    onComplete?.Invoke();
                });
        }

        public void Close(Action onComplete = null)
        {
            if (_isAnimating || !_isOpen)
            {
                onComplete?.Invoke();
                return;
            }

            _isOpen = false;
            _isAnimating = true;

            _currentTween?.Kill();
            _currentTween = panelRect.DOAnchorPos(hiddenPos, slideDuration)
                .SetEase(slideEase)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    _isAnimating = false;
                    onComplete?.Invoke();
                });
        }
    }
}