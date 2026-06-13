using UnityEngine;

namespace _02_Scripts.Map
{
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapDataSO")]
    public class MapDataSO : ScriptableObject
    {
        public MapObjectData[] Objects;
    }

    [System.Serializable]
    public class MapObjectData
    {
        public GameObject Prefab;
        public Vector3 Position;
        public Quaternion Rotation;
    }
}