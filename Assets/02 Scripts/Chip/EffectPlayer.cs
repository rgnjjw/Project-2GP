using System;
using System.Collections.Generic;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Chip
{
    public struct EffectEvent
    {
        public string EffectName;

        public EffectEvent(string effectName)
        {
            EffectName = effectName;
        }
    }
    [Serializable]
    public struct ParticleDict
    {
        public ParticleSystem Particle;
        public string EffectName;
    }

    public class EffectPlayer : MonoBehaviour
    {
        [SerializeField] private ParticleDict[] _particles;
        private Dictionary<string,ParticleSystem> _particleDict;

        private void Awake()    
        {
            _particleDict = new Dictionary<string,ParticleSystem>();
            foreach (var particle in _particles)
            {
                _particleDict.Add(particle.EffectName, particle.Particle);
            }
        }

        private void OnEnable() => EventBus.Subscribe<EffectEvent>(PlayVFX);
        private void OnDisable() => EventBus.Unsubscribe<EffectEvent>(PlayVFX);

        private void PlayVFX(EffectEvent evt)
        {
            _particleDict[evt.EffectName].Play();
        }
    }
}