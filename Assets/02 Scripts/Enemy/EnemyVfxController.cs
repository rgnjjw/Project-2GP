using System;
using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public enum EnemyVfxType
    {
        None,
        HitFlash,
        ExplosionWarmup,
        Explosion,
        MuzzleFlash,
        DeathBurst,
        Shield // 추가
    }

    [Serializable]
    public struct EnemyVfxEntry
    {
        public EnemyVfxType Type;
        public ParticleSystem Particle;
    }

    public class EnemyVfxController : MonoBehaviour, IModule
    {
        [SerializeField] private EnemyVfxEntry[] vfxEntries;

        private Dictionary<EnemyVfxType, ParticleSystem> _vfxDict;

        public void Initialize(ModuleOwner owner)
        {
            _vfxDict = new Dictionary<EnemyVfxType, ParticleSystem>(vfxEntries.Length);
            foreach (var entry in vfxEntries)
            {
                if (entry.Particle == null) continue;
                _vfxDict[entry.Type] = entry.Particle;
                entry.Particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        public void Play(EnemyVfxType type)
        {
            if (!_vfxDict.TryGetValue(type, out var ps) || ps == null) return;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play();
        }

        // 지속형 이펙트(보호막 같은) 끝낼 때 호출
        public void Stop(EnemyVfxType type)
        {
            if (!_vfxDict.TryGetValue(type, out var ps) || ps == null) return;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public ParticleSystem Get(EnemyVfxType type)
            => _vfxDict.TryGetValue(type, out var ps) ? ps : null;
    }
}