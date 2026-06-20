using _02_Scripts.Agent;
using _02_Scripts.Core.Detect;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    // 힐 대상/추격 대상 선정 로직을 한 곳에 모은다.
    // 태그·레이어가 아니라 Enemy 컴포넌트의 스킬 구성으로 "힐러"를 식별한다.
    public static class HealTargeting
    {
        // 힐러 = 힐 스킬을 하나라도 가진 적.
        public static bool IsHealer(Enemy enemy)
        {
            var controller = enemy.GetModule<EnemySkillController>();
            if (controller == null) return false;

            foreach (var skill in controller.Skills)
                if (skill != null && skill.IsHealSkill) return true;

            return false;
        }

        // 유효한 힐 대상: 자기 자신 X, 죽은 적 X, 다른 힐러 X, 풀피 X → 살아있는 다친 일반 아군만 O.
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

        // 범위 안에서 체력이 가장 낮은(동률이면 가장 가까운) 유효 아군.
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
