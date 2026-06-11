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

            if (_rtIncreasing)
                followBar.fillAmount = amount;
            else
                bar.fillAmount = amount;
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

        public void SetFill(float targetAmount)
        {
            bar.DOKill();
            followBar.DOKill();

            bool isIncreasing = targetAmount > bar.fillAmount;

            if (isIncreasing)
            {
                followBar.DOFillAmount(targetAmount, followDuration).SetEase(ease);
                bar.DOFillAmount(targetAmount, barDuration).SetEase(ease).SetDelay(followDelay)
                    .OnComplete(() =>
                    {
                        if (Mathf.Approximately(bar.fillAmount, 1f))
                        {
                            bar.fillAmount = 0f;
                            followBar.fillAmount = 0f;
                        }
                    });
            }
            else
            {
                bar.DOFillAmount(targetAmount, barDuration).SetEase(ease);
                followBar.DOFillAmount(targetAmount, followDuration).SetEase(ease).SetDelay(followDelay);
            }
        }
    }
}