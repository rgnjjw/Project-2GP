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
        Explosion,
        MuzzleFlash,
        Shield,
        AreaHealEffect,
        HealedEffect,
        GravityZone,

        // 아래는 끝에 추가(기존 직렬화 값 보존)
        MeleeAttack,
        Dash,
        Summon,
        SingleHealEffect,
        EarthQuake
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

        public void Stop(EnemyVfxType type)
        {
            if (!_vfxDict.TryGetValue(type, out var ps) || ps == null) return;
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        public ParticleSystem Get(EnemyVfxType type)
            => _vfxDict.TryGetValue(type, out var ps) ? ps : null;
    }
}