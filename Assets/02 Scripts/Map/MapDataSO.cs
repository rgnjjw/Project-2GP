using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Map
{
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "Map/MapDataSO", order = 0)]
    public class MapDataSO : ScriptableObject
    {
        public List<MapObjectData> MapObjectList =  new List<MapObjectData>();
    }
}