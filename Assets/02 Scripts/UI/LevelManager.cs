using System;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.UI
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [SerializeField] private BarUI levelBar;
        [SerializeField] private int[] expPerLevel;

        public event Action<int> OnLevelUp;

        public int CurrentLevel => _currentLevel;
        public float ExpMultiplier { get; set; } = 1f;

        private int _currentLevel = 1;
        private int _currentExp;

        public void AddExp(int amount)
        {
            if (_currentLevel > expPerLevel.Length) return;

            _currentExp += Mathf.RoundToInt(amount * ExpMultiplier);

            int levelsGained = 0;
            while (_currentLevel <= expPerLevel.Length
                   && _currentExp >= expPerLevel[_currentLevel - 1])
            {
                _currentExp -= expPerLevel[_currentLevel - 1];
                _currentLevel++;
                levelsGained++;
            }

            float fill = _currentLevel > expPerLevel.Length
                ? 1f
                : (float)_currentExp / expPerLevel[_currentLevel - 1];

            if (levelsGained > 0)
            {
                int startLevel = _currentLevel - levelsGained;
                levelBar.QueueLevelUps(levelsGained, fill, () =>
                {
                    startLevel++;
                    OnLevelUp?.Invoke(startLevel);
                });
            }
            else
            {
                levelBar.SetFill(fill);
            }
        }
    }
}