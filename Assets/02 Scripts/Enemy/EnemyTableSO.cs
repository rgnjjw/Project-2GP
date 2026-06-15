using UnityEngine;

namespace _02_Scripts.Enemy
{
    [CreateAssetMenu(fileName = "EnemyTableSO", menuName = "Enemy/EnemyTableSO")]
    public class EnemyTableSO : ScriptableObject
    {
        public EnemyLevelData[] LevelTable;

        public EnemyLevelData GetData(int currentLevel)
        {
            foreach (var entry in LevelTable)
            {
                if (currentLevel >= entry.minLevel && currentLevel <= entry.maxLevel)
                    return entry;
            }
            return LevelTable.Length > 0 ? LevelTable[LevelTable.Length - 1] : null;
        }
    }

    [System.Serializable]
    public class EnemyLevelData
    {
        public int minLevel;
        public int maxLevel;
        public GameObject[] meleePrefabs;
        public GameObject[] rangedPrefabs;
        public GameObject[] bossPrefabs;
    }
}
