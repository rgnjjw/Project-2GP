using Unity.Cinemachine;
using UnityEngine;

namespace _02_Scripts.Gun
{
    [CreateAssetMenu(fileName = "RecoilDataSO", menuName = "Gun/RecoilDataSO")]
    public class RecoilDataSO : ScriptableObject
    {
        [field: SerializeField] public float Force { get; private set; } = 1f;
        [field: SerializeField] public Vector3 Direction { get; private set; } = new Vector3(0f, 0.5f, -1f);
        [field: SerializeField] public float Duration { get; private set; } = 0.2f;
        [field: SerializeField] public CinemachineImpulseDefinition.ImpulseShapes ImpulseShape { get; private set; } = CinemachineImpulseDefinition.ImpulseShapes.Bump;
    }
}