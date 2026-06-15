using UnityEngine;

namespace _02_Scripts.Map
{
    [CreateAssetMenu(fileName = "WaveDataSO", menuName = "Map/WaveDataSO")]
    public class WaveDataSO : ScriptableObject
    {
        public int meleeCount;
        public int rangedCount;
        public bool spawnBoss;
    }
}
