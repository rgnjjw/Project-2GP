using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    // 힐 스킬 공통 베이스. 대상 판정은 HealTargeting에 위임한다.
    // (태그·레이어가 아니라 Enemy 컴포넌트의 스킬 구성으로 "힐러"를 식별한다.)
    public abstract class HealSkillSO : SkillSO
    {
        public override bool IsHealSkill => true;

        // 유효한 힐 대상인지: 자기 자신 X, 죽은 적 X, 다른 힐러 X, 풀피 X → 살아있는 다친 일반 아군만 O.
        protected bool IsValidTarget(Enemy self, Transform candidate, out Enemy ally)
            => HealTargeting.IsValidTarget(self, candidate, out ally);

        // 사거리 안에 유효한 힐 대상이 하나라도 있어야 스킬 발동(힐러끼리만/풀피만 있으면 발동 안 함).
        public override bool CanExecuteSkill(Enemy enemy)
        {
            if (TargetFinder == null) return false;

            foreach (var t in TargetFinder.GetAllInRange(enemy.transform))
                if (IsValidTarget(enemy, t, out _)) return true;

            return false;
        }
    }
}
