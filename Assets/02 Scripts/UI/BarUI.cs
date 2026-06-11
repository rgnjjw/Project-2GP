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

        [Header("Realtime (Stamina)")]
        [SerializeField] private float frontSpeed = 10f;
        [SerializeField] private float backSpeed = 8f;

        private float _targetFill;

        private void Awake()
        {
            _targetFill = bar.fillAmount;
        }

        public void SetFillRealtime(float amount)
        {
            _targetFill = Mathf.Clamp01(amount);
        }

        private void Update()
        {
            if (bar.fillAmount < _targetFill)
            {
                followBar.fillAmount = Mathf.MoveTowards(
                    followBar.fillAmount,
                    _targetFill,
                    frontSpeed * Time.deltaTime);

                bar.fillAmount = Mathf.MoveTowards(
                    bar.fillAmount,
                    followBar.fillAmount,
                    backSpeed * Time.deltaTime);
            }
            else
            {
                bar.fillAmount = Mathf.MoveTowards(
                    bar.fillAmount,
                    _targetFill,
                    frontSpeed * Time.deltaTime);

                followBar.fillAmount = Mathf.MoveTowards(
                    followBar.fillAmount,
                    bar.fillAmount,
                    backSpeed * Time.deltaTime);
            }
        }

        public void SetFill(float targetAmount)
        {
            bar.DOKill();
            followBar.DOKill();

            bool isIncreasing = targetAmount > bar.fillAmount;

            if (isIncreasing)
            {
                followBar.DOFillAmount(targetAmount, followDuration)
                    .SetEase(ease);

                bar.DOFillAmount(targetAmount, barDuration)
                    .SetEase(ease)
                    .SetDelay(followDelay)
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
                bar.DOFillAmount(targetAmount, barDuration)
                    .SetEase(ease);

                followBar.DOFillAmount(targetAmount, followDuration)
                    .SetEase(ease)
                    .SetDelay(followDelay);
            }
        }
    }
}