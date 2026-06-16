using System;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.UI.StyleBar
{
    public enum StyleAction
    {
        Kill,
        Headshot,
        DashKill,
        HighJump,
        FastMove
    }
    public enum StyleGrade { D, C, B, A, S }
    public class StyleManager : MonoSingleton<StyleManager>
    {
        [SerializeField] private StyleScoreDataSO scoreData;
        [SerializeField] private float decayDelay = 3f;//감소되기까지 기다리는 시간 아무것도 안하면 그때부터 초셈
        [SerializeField] private float decayRate = 10f;//초당 감소량

        public event Action<StyleGrade> OnGradeChanged;
        public event Action<float> OnScoreChanged;

        public StyleGrade CurrentGrade { get; private set; } = StyleGrade.D;
        public float CurrentScore { get; private set; } = 0f;

        private float _lastActionTime;

        private void Update()
        {
            if (Time.time - _lastActionTime < decayDelay) return;
            AddScore(-decayRate * Time.deltaTime);
        }

        public void AddStyleScore(StyleAction action)
        {
            float score = action switch
            {
                StyleAction.Kill => scoreData.KillScore,
                StyleAction.Headshot => scoreData.HeadshotScore,
                StyleAction.DashKill => scoreData.DashKillScore,
                StyleAction.HighJump => scoreData.HighJumpScore,
                StyleAction.FastMove => scoreData.FastMoveScore,
                _ => 0f
            };
            AddScore(score);
            NotifyAction();
        }

        private void AddScore(float amount)
        {
            CurrentScore += amount;

            if (CurrentScore >= 100f)
            {
                if (CurrentGrade == StyleGrade.S)
                {
                    CurrentScore = 100f;
                }
                else
                {
                    CurrentGrade++;
                    CurrentScore = 0f;
                    OnGradeChanged?.Invoke(CurrentGrade);
                }
            }
            else if (CurrentScore < 0f)
            {
                if (CurrentGrade == StyleGrade.D)
                {
                    CurrentScore = 0f;
                }
                else
                {
                    CurrentGrade--;
                    CurrentScore = 100f;
                    OnGradeChanged?.Invoke(CurrentGrade);
                }
            }

            OnScoreChanged?.Invoke(CurrentScore / 100f);
        }

        private void NotifyAction()
        {
            _lastActionTime = Time.time;
        }
    }
}