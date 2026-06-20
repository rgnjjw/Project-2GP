using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "SingleHealSkillSO", menuName = "Skill/SingleHealSkillSO", order = 0)]
    public class SingleHealSkillSO : SkillSO
    {
        [field: SerializeField] public int HealAmount { get; private set; } = 30;

        private EnemyAnimationEvent _animEvent;
        private Enemy _enemy;
        private Transform _healTarget;

        public override void ExecuteSkill(Enemy enemy)
        {
            _enemy = enemy;
            _healTarget = SelectHealTarget(enemy);
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();

            _animEvent.OnAttack += HandleAttack;
        }

        private void HandleAttack()
        {
            _animEvent.OnAttack -= HandleAttack;

            if (_healTarget != null && _healTarget.TryGetComponent<Enemy>(out var ally))
                ally.GetModule<AgentHealth>()?.ApplyHeal(HealAmount);

            // _vfx?.Play(EnemyVfxType.None); 이건 힐받은놈이 이펙트로 교체함

            NotifyComplete();
        }

        private Transform SelectHealTarget(Enemy enemy)
        {
            if (TargetFinder == null) return null;

            Transform best = null;
            int lowestHp = int.MaxValue;
            float bestDist = float.MaxValue;

            foreach (var t in TargetFinder.GetAllInRange(enemy.transform))
            {
                if (!t.TryGetComponent<Enemy>(out var ally)) continue;

                var health = ally.GetModule<AgentHealth>();
                if (health == null || health.IsDead) continue;

                int hp = health.CurrentHp.Value;
                float dist = Vector3.Distance(enemy.transform.position, t.position);

                bool isLower = hp < lowestHp;
                bool isSameHpButCloser = hp == lowestHp && dist < bestDist;

                if (isLower || isSameHpButCloser)
                {
                    lowestHp = hp;
                    bestDist = dist;
                    best = t;
                }
            }

            return best;
        }
    }
}