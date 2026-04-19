using UnityEngine;

namespace _02_Scripts.Interface
{
    public interface IRenderer
    {
        Animator Animator { get; }
        void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0);
    }
}