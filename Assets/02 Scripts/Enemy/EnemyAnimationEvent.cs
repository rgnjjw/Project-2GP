using System;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class EnemyAnimationEvent : MonoBehaviour,IModule
    {
        public event Action OnAttackEnd;
        public event Action OnAttack;
        public event Action OnDeath;
        public event Action OnHitEnd;
        public void HitEnd() => OnHitEnd?.Invoke();
        public void Initialize(ModuleOwner owner) { }
        
        public void AttackEnd() => OnAttackEnd?.Invoke();

        public void Attack() => OnAttack?.Invoke();
        
        public void Death() => OnDeath?.Invoke();
    }
}