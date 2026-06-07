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

        private int _currentLevel = 1;
        private int _currentExp;

        public void AddExp(int amount)
        {
            if (_currentLevel > expPerLevel.Length) return; 

            _currentExp += amount;

            while (_currentLevel <= expPerLevel.Length && _currentExp >= expPerLevel[_currentLevel - 1])
            {
                _currentExp -= expPerLevel[_currentLevel - 1];
                _currentLevel++;
                OnLevelUp?.Invoke(_currentLevel);
            }

            UpdateBar();
        }

        private void UpdateBar()
        {
            if (_currentLevel - 1 >= expPerLevel.Length) return;

            float ratio = (float)_currentExp / expPerLevel[_currentLevel - 1];
            levelBar.SetFill(ratio);
        }
    }
}