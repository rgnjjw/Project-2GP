using _02_Scripts.Agent;
using _02_Scripts.Core.Detect;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    public static class HealTargeting
    {
        public static bool IsHealer(Enemy enemy)
        {
            var controller = enemy.GetModule<EnemySkillController>();
            if (controller == null) return false;

            foreach (var skill in controller.Skills)
                if (skill != null && skill.IsHealSkill) return true;

            return false;
        }

        public static bool IsValidTarget(Enemy self, Transform candidate, out Enemy ally)
        {
            ally = null;
            if (candidate == self.transform) return false;
            if (!candidate.TryGetComponent(out ally)) return false;

            var health = ally.GetModule<AgentHealth>();
            if (health == null || health.IsDead) return false;
            if (health.CurrentHp.Value >= health.MaxHp) return false; // 다친 아군만

            if (IsHealer(ally)) return false;

            return true;
        }

        public static Transform GetLowestHp(Enemy self, AbstractDetection range)
        {
            if (range == null) return null;

            Transform best = null;
            int lowestHp = int.MaxValue;
            float bestDist = float.MaxValue;

            foreach (var t in range.GetAllInRange(self.transform))
            {
                if (!IsValidTarget(self, t, out var ally)) continue;

                int hp = ally.GetModule<AgentHealth>().CurrentHp.Value;
                float dist = (self.transform.position - t.position).sqrMagnitude;

                if (hp < lowestHp || (hp == lowestHp && dist < bestDist))
                {
                    lowestHp = hp;
                    bestDist = dist;
                    best = t;
                }
            }

            return best;
        }

        // 추격 대상 선정: 힐러는 체력이 가장 낮은 아군을 우선 추격(다친 아군이 없으면 가장 가까운 아군).
        // 일반 적은 기존대로 가장 가까운 대상(=플레이어)을 추격한다.
        public static Transform SelectChaseTarget(Enemy enemy, AbstractDetection range)
        {
            if (range == null) return null;

            if (IsHealer(enemy))
                return GetLowestHp(enemy, range) ?? range.GetClosest(enemy.transform);

            return range.GetClosest(enemy.transform);
        }
    }
}
