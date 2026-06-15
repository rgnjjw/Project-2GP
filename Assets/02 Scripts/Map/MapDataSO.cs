using _02_Scripts.Enemy;
using UnityEngine;

namespace _02_Scripts.Map
{
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapDataSO")]
    public class MapDataSO : ScriptableObject
    {
        public MapObjectData[] Objects;
        public SpawnPointData[] SpawnPoints;
        public WaveDataSO[] Waves;
    }

    [System.Serializable]
    public class MapObjectData
    {
        public GameObject Prefab;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    [System.Serializable]
    public class SpawnPointData
    {
        public EnemyType Type;
        public Vector3 Position;
    }
}
