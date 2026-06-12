using System;
using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Player
{
    [Serializable]
    struct PlayerVFXInfo
    {
        public string Key;
        public ParticleSystem Value;
    }

    public class PlayerVFXContainer : MonoBehaviour, IModule
    {
        [SerializeField] private PlayerVFXInfo[] vfxInfos;
        private Dictionary<string, ParticleSystem> _vfxDict;

        public void Initialize(ModuleOwner owner)
        {
            _vfxDict = new Dictionary<string, ParticleSystem>();
            foreach (var info in vfxInfos)
                _vfxDict.TryAdd(info.Key, info.Value);
        }

        public void Play(string key)
        {
            if (_vfxDict.TryGetValue(key, out var ps))
                ps.Play();
        }

        public void Stop(string key)
        {
            if (_vfxDict.TryGetValue(key, out var ps))
                ps.Stop();
        }
    }
}