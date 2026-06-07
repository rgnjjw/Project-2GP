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