using System;
using _02_Scripts.Core.AnimationSystem;
using _02_Scripts.Core.Detect;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    public abstract class SkillSO : ScriptableObject
    {
        [field: SerializeField] public AnimParamSO AnimParam { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }

        [SerializeReference] public AbstractDetection DamageAreaDetection;
        [SerializeReference] public AbstractDetection TargetFinder;

        public event Action<Enemy> OnExecutionComplete;

        public virtual bool IsHealSkill => false;

        public virtual bool CanExecuteSkill(Enemy enemy)
            => TargetFinder != null && TargetFinder.HasAnyInRange(enemy.transform);

        public abstract void ExecuteSkill(Enemy enemy);

        protected void NotifyComplete(Enemy enemy) => OnExecutionComplete?.Invoke(enemy);
    }
}