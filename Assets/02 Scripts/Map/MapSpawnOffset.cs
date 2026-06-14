using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapSpawnOffset : MonoBehaviour
    {
        [field: SerializeField] public float SpawnOffsetY { get; private set; } = 20f;
    }
}