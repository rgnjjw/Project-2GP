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
        
        public bool CanExecuteSkill(Transform centerTrm)
            => TargetFinder != null && TargetFinder.HasAnyInRange(centerTrm);

        public abstract void ExecuteSkill(Transform centerTrm);
    }
}