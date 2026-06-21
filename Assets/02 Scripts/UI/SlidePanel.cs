using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    public class SlidePanel : MonoBehaviour
    {
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private Vector2 hiddenPos;
        [SerializeField] private Vector2 shownPos;
        [SerializeField] private float slideDuration = 0.3f;
        [SerializeField] private Ease slideEase = Ease.OutCubic;

        [Header("Auto Close")]
        [SerializeField] private bool closeOnEscape;
        [SerializeField] private bool closeOnClickOutside;
        [Tooltip("이 영역들을 클릭하면 바깥 클릭으로 간주하지 않습니다 (예: 패널을 여는 버튼).")]
        [SerializeField] private List<RectTransform> ignoreRects = new();
        [SerializeField ] private Image blackPanel;

        private bool _isOpen;
        private bool _isAnimating;
        private Tween _currentTween;
        private int _openedFrame = -1;
        private Camera _eventCamera;
        private bool _eventCameraResolved;

        public bool IsOpen => _isOpen;

        private void Awake()
        {
            // 시작 시 항상 닫힌 상태로 초기화 (위치는 화면 밖, 검은 배경은 투명)
            _isOpen = false;
            panelRect.anchoredPosition = hiddenPos;

            if (blackPanel != null)
            {
                var c = blackPanel.color;
                c.a = 0f;
                blackPanel.color = c;
                blackPanel.raycastTarget = false;
            }
        }

        private void Update()
        {
            if (!_isOpen) return;

            if (closeOnEscape
                && Keyboard.current != null
                && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Close();
                return;
            }

            if (closeOnClickOutside
                && Mouse.current != null
                && Mouse.current.leftButton.wasPressedThisFrame
                && Time.frameCount != _openedFrame
                && IsPointerOutside(Mouse.current.position.ReadValue()))
            {
                Close();
            }
        }

        private bool IsPointerOutside(Vector2 screenPos)
        {
            var cam = GetEventCamera();

            if (RectTransformUtility.RectangleContainsScreenPoint(panelRect, screenPos, cam))
                return false;

            foreach (var rect in ignoreRects)
            {
                if (rect != null && RectTransformUtility.RectangleContainsScreenPoint(rect, screenPos, cam))
                    return false;
            }

            return true;
        }

        private Camera GetEventCamera()
        {
            if (_eventCameraResolved) return _eventCamera;

            var canvas = panelRect != null ? panelRect.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                _eventCamera = canvas.worldCamera;

            _eventCameraResolved = true;
            return _eventCamera;
        }

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
            
            blackPanel?.DOFade(0.7f, slideDuration);
            if (blackPanel != null) blackPanel.raycastTarget = true;

            _isOpen = true;
            _isAnimating = true;
            _openedFrame = Time.frameCount;

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

            blackPanel?.DOFade(0, slideDuration);
            if (blackPanel != null) blackPanel.raycastTarget = false;

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
