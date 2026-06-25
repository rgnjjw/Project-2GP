using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    public abstract class HealSkillSO : SkillSO
    {
        public override bool IsHealSkill => true;

        protected bool IsValidTarget(Enemy self, Transform candidate, out Enemy ally)
            => HealTargeting.IsValidTarget(self, candidate, out ally);

        public override bool CanExecuteSkill(Enemy enemy)
        {
            if (TargetFinder == null) return false;

            foreach (var t in TargetFinder.GetAllInRange(enemy.transform))
                if (IsValidTarget(enemy, t, out _)) return true;

            return false;
        }
    }
}
