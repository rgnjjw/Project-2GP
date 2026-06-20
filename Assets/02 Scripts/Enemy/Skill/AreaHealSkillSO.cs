using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "AreaHealSkillSO", menuName = "Skill/AreaHealSkillSO", order = 0)]
    public class AreaHealSkillSO : SkillSO
    {
        [field: SerializeField] public int HealAmount { get; private set; } = 20;
        [SerializeField] private bool includeSelf = true;

        private EnemyAnimationEvent _animEvent;
        private EnemyVfxController _vfx;
        private Enemy _enemy;

        public override void ExecuteSkill(Enemy enemy)
        {
            _enemy = enemy;
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();
            _vfx = enemy.GetModule<EnemyVfxController>();

            _animEvent.OnAttack += HandleAttack;
        }

        private void HandleAttack()
        {
            _animEvent.OnAttack -= HandleAttack;
            HealAllInRange();
            NotifyComplete();
        }

        private void HealAllInRange()
        {
            if (includeSelf)
                _enemy.GetModule<AgentHealth>()?.ApplyHeal(HealAmount);

            if (DamageAreaDetection != null)
            {
                foreach (var t in DamageAreaDetection.GetAllInRange(_enemy.transform))
                {
                    if (t == _enemy.transform) continue; 

                    if (t.TryGetComponent<Enemy>(out var ally))
                        ally.GetModule<AgentHealth>()?.ApplyHeal(HealAmount);
                }
            }

            _vfx?.Play(EnemyVfxType.None);//힐 이펙트
        }
    }
}