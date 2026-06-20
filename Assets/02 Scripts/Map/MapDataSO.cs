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

        [Header("타입 구성 가중치 (이 맵의 근거리:원거리:보스 비율)")]
        [Tooltip("근거리 가중치. 예: 근거리7, 원거리3, 보스0 이면 총 적의 70%가 근거리.")]
        [Min(0f)] public float meleeWeight = 1f;
        [Tooltip("원거리 가중치.")]
        [Min(0f)] public float rangedWeight = 1f;
        [Tooltip("보스 가중치. 0이면 이 맵엔 보스가 안 나온다.")]
        [Min(0f)] public float bossWeight = 0f;
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
