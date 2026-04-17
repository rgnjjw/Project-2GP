using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapRoot : MonoBehaviour
    {
        [field: SerializeField] public MapDataSO TargetMapDataSO { get; set; }
    }
}