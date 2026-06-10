using _02_Scripts.Core.Interface;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Agent
{
    [RequireComponent(typeof(Animator))]
    public class AgentRenderer : MonoBehaviour, IModule, IRenderer
    {
        public Animator Animator { get; private set; }
        

        public virtual void Initialize(ModuleOwner owner)
        {
            Animator = GetComponent<Animator>();
        }

        public void PlayClip(int clipHash, float normalizedTime, float crossFadeDuration, int layerIndex = 0)
        {
            Animator.CrossFadeInFixedTime(clipHash, crossFadeDuration, layerIndex, normalizedTime);
        }
    }
}