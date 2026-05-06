using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Map
{
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapDataSO", order = 0)]
    public class MapDataSO : ScriptableObject
    {
        public MapObjectData[] MapObjectList;
        public EnemySpawnData[] EnemySpawnData;
    }

    [Serializable]
    public class EnemySpawnData
    {
        public Vector3 enemySpawnPoint;
        public GameObject enemyPrefab;
    }
}