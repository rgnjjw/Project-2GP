using System;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.UI
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [SerializeField] private BarUI levelBar;
        [SerializeField] private int[] expPerLevel; // 레벨별 필요한 경험치의 양
        
        public event Action<int> OnLevelUp;

        public int CurrentLevel => _currentLevel;
        public float ExpMultiplier { get; set; } = 1f;
        private int _currentLevel = 1;
        private int _currentExp;

        public void AddExp(int amount)
        {
            if (_currentLevel > expPerLevel.Length) return;

            _currentExp += Mathf.RoundToInt(amount * ExpMultiplier);
            ProcessExp();
        }

        private void ProcessExp()
        {
            if (_currentLevel > expPerLevel.Length) return;

            int needed = expPerLevel[_currentLevel - 1];

            if (_currentExp >= needed)
            {
                levelBar.SetFill(1f, () =>
                {
                    _currentExp -= needed;
                    _currentLevel++;
                    OnLevelUp?.Invoke(_currentLevel);
                    ProcessExp();
                });
            }
            else
            {
                levelBar.SetFill((float)_currentExp / needed);
            }
        }
    }
}