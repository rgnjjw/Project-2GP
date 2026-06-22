using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    public class BarUI : MonoBehaviour
    {
        [SerializeField] private Image bar;
        [SerializeField] private Image followBar;
        [SerializeField] private float followDelay = 0.3f;
        [SerializeField] private float barDuration = 0.5f;
        [SerializeField] private float followDuration = 0.4f;
        [SerializeField] private Ease ease = Ease.Linear;

        private float _lastTarget = -1f;
        private bool _isLevelUpAnimating;
        private Sequence _levelUpSeq;

        private readonly Queue<Action> _levelUpCallbacks = new();
        private float _pendingFillAfter;

        // ── Realtime (스태미나 등) ──────────────────────────────────────────
        private float _rtTarget = -1f;
        private bool _rtIncreasing;
        private float _rtLagStart;
        private bool _rtLagActive;

        public void SetFillRealtime(float amount)
        {
            amount = Mathf.Clamp01(amount);

            if (_rtTarget < 0f)
            {
                bar.fillAmount = amount;
                followBar.fillAmount = amount;
                _rtTarget = amount;
                return;
            }

            bool nowIncreasing = amount > _rtTarget;
            bool nowDecreasing = amount < _rtTarget;
            bool directionChanged = (nowIncreasing && !_rtIncreasing) || (nowDecreasing && _rtIncreasing);

            if ((nowIncreasing || nowDecreasing) && (!_rtLagActive || directionChanged))
            {
                _rtIncreasing = nowIncreasing;
                _rtLagStart = Time.time;
                _rtLagActive = true;
            }

            _rtTarget = amount;

            if (_rtIncreasing) followBar.fillAmount = amount;
            else bar.fillAmount = amount;
        }

        private void Update()
        {
            if (!_rtLagActive) return;
            if (Time.time - _rtLagStart < followDelay) return;

            float speed = 1f / (_rtIncreasing ? barDuration : followDuration);
            float step = speed * Time.deltaTime;

            if (_rtIncreasing)
                bar.fillAmount = Mathf.MoveTowards(bar.fillAmount, _rtTarget, step);
            else
                followBar.fillAmount = Mathf.MoveTowards(followBar.fillAmount, _rtTarget, step);
        }

        // ── 일반 fill (순수 시각 효과, 게임 로직과 무관) ──────────────────────
        public void SetFill(float targetAmount)
        {
            // 레벨업 연출이 진행 중이면 끼어들지 않는다.
            // (예전엔 여기서 시퀀스를 죽이고 콜백을 비워버려서, 연출 도중 경험치가 더 들어오면
            //  '레벨업 카드' 콜백이 사라져 카드가 가끔 무시됐음 → 간헐적 버그의 원인)
            // 진행 중에 들어온 경험치는 최종 fill만 갱신해두고 연출이 끝날 때 반영된다.
            if (_isLevelUpAnimating)
            {
                _pendingFillAfter = targetAmount;
                return;
            }

            bool wasLevelUp = _isLevelUpAnimating;
            _isLevelUpAnimating = false;
            _levelUpCallbacks.Clear();

            _levelUpSeq?.Kill();
            _levelUpSeq = null;
            bar.DOKill();
            followBar.DOKill();

            if (wasLevelUp)
            {
                followBar.fillAmount = bar.fillAmount;
                _lastTarget = bar.fillAmount;
            }

            bool isIncreasing = _lastTarget < 0f || targetAmount >= _lastTarget;
            _lastTarget = targetAmount;

            if (isIncreasing)
            {
                followBar.DOFillAmount(targetAmount, followDuration).SetEase(ease);
                bar.DOFillAmount(targetAmount, barDuration).SetEase(ease).SetDelay(followDelay);
            }
            else
            {
                bar.DOFillAmount(targetAmount, barDuration).SetEase(ease);
                followBar.DOFillAmount(targetAmount, followDuration).SetEase(ease).SetDelay(followDelay);
            }
        }

        // ── 레벨업 시각 효과 ───────────────────────────────────────────────
        /// <summary>
        /// count번 "꽉 차고 리셋"을 순서대로 재생한 뒤 finalFill까지 채운다.
        /// onEachLevelUp은 각 climb이 끝나고 0으로 리셋된 직후(=실제 레벨업 처리 시점)에 호출된다.
        /// </summary>
        public void QueueLevelUps(int count, float finalFill, Action onEachLevelUp = null)
        {
            if (count <= 0)
            {
                SetFill(finalFill);
                return;
            }

            for (int i = 0; i < count; i++)
                _levelUpCallbacks.Enqueue(onEachLevelUp);

            _pendingFillAfter = finalFill;

            if (!_isLevelUpAnimating)
                PlayNextLevelUp();
        }

        private void PlayNextLevelUp()
        {
            _isLevelUpAnimating = true;

            Action callback = _levelUpCallbacks.Count > 0 ? _levelUpCallbacks.Dequeue() : null;

            bar.DOKill();
            followBar.DOKill();

            followBar.fillAmount = bar.fillAmount;
            _lastTarget = 1f;

            _levelUpSeq = DOTween.Sequence();
            _levelUpSeq.Join(bar.DOFillAmount(1f, barDuration).SetEase(ease));
            _levelUpSeq.Join(followBar.DOFillAmount(1f, barDuration).SetEase(ease));
            _levelUpSeq.OnComplete(() =>
            {
                _levelUpSeq = null;
                bar.fillAmount = 0f;
                followBar.fillAmount = 0f;
                _lastTarget = 0f;

                // 바가 꽉 찼다가 0으로 리셋된 "직후"에 실제 레벨업 이벤트 발동
                callback?.Invoke();

                if (_levelUpCallbacks.Count > 0)
                {
                    PlayNextLevelUp();
                }
                else
                {
                    _isLevelUpAnimating = false;
                    SetFill(_pendingFillAfter);
                }
            });
        }
    }
}