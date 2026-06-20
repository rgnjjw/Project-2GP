using System;
using _02_Scripts.Core.AnimationSystem;
using _02_Scripts.Core.Detect;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    public abstract class SkillSO : ScriptableObject
    {
        //이펙트 연결하기
        [field: SerializeField] public AnimParamSO AnimParam { get; private set; }
        [field: SerializeField] public float Cooldown { get; private set; }

        [SerializeReference] public AbstractDetection DamageAreaDetection;
        [SerializeReference] public AbstractDetection TargetFinder;

        public event Action OnExecutionComplete;

        // 힐 스킬 여부(힐러 식별에 사용). 힐 스킬에서 true로 오버라이드한다.
        public virtual bool IsHealSkill => false;

        public virtual bool CanExecuteSkill(Enemy enemy)
            => TargetFinder != null && TargetFinder.HasAnyInRange(enemy.transform);

        public abstract void ExecuteSkill(Enemy enemy);

        protected void NotifyComplete() => OnExecutionComplete?.Invoke();
    }
}