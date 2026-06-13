using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapRoot : MonoBehaviour
    {
        [field: SerializeField] public MapDataSO TargetData { get; private set; }
    }
}