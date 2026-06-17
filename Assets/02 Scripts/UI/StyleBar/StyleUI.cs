using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _02_Scripts.UI.StyleBar
{
    public class StyleUI : MonoBehaviour
    {
        [SerializeField] private BarUI barUI;
        [SerializeField] private TMP_Text gradeText;
        [SerializeField] private StyleFeedUI feedUI;

        [Header("Grade Colors")]
        [SerializeField] private Color gradeColorD = Color.white;
        [SerializeField] private Color gradeColorC = Color.green;
        [SerializeField] private Color gradeColorB = Color.blue;
        [SerializeField] private Color gradeColorA = Color.yellow;
        [SerializeField] private Color gradeColorS = Color.red;

        private void Awake()
        {
            StyleManager.Instance.OnGradeChanged += UpdateGrade;
            StyleManager.Instance.OnScoreChanged += UpdateScore;
            StyleManager.Instance.OnStyleAction += UpdateFeed;
            UpdateGrade(StyleManager.Instance.CurrentGrade);
        }

        private void OnDestroy()
        {
            StyleManager.Instance.OnGradeChanged -= UpdateGrade;
            StyleManager.Instance.OnScoreChanged -= UpdateScore;
            StyleManager.Instance.OnStyleAction -= UpdateFeed;
        }

        private void UpdateGrade(StyleGrade grade)
        {
            gradeText.text = "Grade : " + grade.ToString();
            gradeText.color = grade switch
            {
                StyleGrade.D => gradeColorD,
                StyleGrade.C => gradeColorC,
                StyleGrade.B => gradeColorB,
                StyleGrade.A => gradeColorA,
                StyleGrade.S => gradeColorS,
                _ => Color.white
            };

            gradeText.transform.DOKill(true);

            if (grade == StyleGrade.S)
            {
                gradeText.transform.DOScale(Vector3.one * 1.3f, 0.3f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .SetUpdate(true);
            }
            else
            {
                gradeText.transform.localScale = Vector3.one;
                gradeText.transform.DOPunchScale(Vector3.one * 0.5f, 0.4f, 0, 0.5f).SetUpdate(true);
                //커졌다 다시 원래의 크기로 돌아오는 DOPunchScale 함수 50퍼센트 커졌다가 다시 원래대로됨
                //vibrato는 튕기는 횟수인데 기본값은 10이고 0이면 1번만 튕기고 돌아옴 범위는 0서 1까지고 탄성력 있게 돌아옴
            }
        }

        private void UpdateScore(float ratio)
        {
            barUI.SetFillRealtime(ratio);
        }

        private void UpdateFeed(string message)
        {
            feedUI.AddFeed(message);
        }
    }
}