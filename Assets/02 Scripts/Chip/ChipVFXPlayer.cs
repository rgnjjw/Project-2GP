using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Chip
{
    public struct ChipVFXEvent { }
    public class ChipVFXPlayer : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;
        private void OnEnable() => EventBus.Subscribe<ChipVFXEvent>(PlayVFX);
        private void OnDisable() => EventBus.Unsubscribe<ChipVFXEvent>(PlayVFX);

        private void PlayVFX(ChipVFXEvent evt)
        {
            particle.Play();
        }
    }
}