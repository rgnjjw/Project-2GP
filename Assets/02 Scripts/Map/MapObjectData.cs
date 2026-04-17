using System;
using UnityEngine;

namespace _02_Scripts.Map
{
    [Serializable]
    public class MapObjectData
    {
        public GameObject prefab;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
}
