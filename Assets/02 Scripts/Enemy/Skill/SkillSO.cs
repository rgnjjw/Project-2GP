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

        // SkillSO는 ScriptableObject라 같은 종류의 적들이 '하나의 에셋 인스턴스'를 공유한다.
        // 따라서 완료 이벤트에 어느 적이 끝났는지를 실어 보내야, 한 적의 스킬 완료가
        // 다른 적의 공격 상태까지 종료시키는 교차 오발(적 행동이 꼬이는 버그)을 막을 수 있다.
        public event Action<Enemy> OnExecutionComplete;

        // 힐 스킬 여부(힐러 식별에 사용). 힐 스킬에서 true로 오버라이드한다.
        public virtual bool IsHealSkill => false;

        public virtual bool CanExecuteSkill(Enemy enemy)
            => TargetFinder != null && TargetFinder.HasAnyInRange(enemy.transform);

        public abstract void ExecuteSkill(Enemy enemy);

        protected void NotifyComplete(Enemy enemy) => OnExecutionComplete?.Invoke(enemy);
    }
}