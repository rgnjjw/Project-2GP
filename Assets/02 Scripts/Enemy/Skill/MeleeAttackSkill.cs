using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "MeleeAttackSkill", menuName = "Skill/MeleeAttackSkill", order = 0)]
    public class MeleeAttackSkill : SkillSO
    {
        [field: SerializeField] public int Damage { get;private set; }
        public override void ExecuteSkill(Transform centerTrm)
        {
            var closest = DamageAreaDetection.GetClosest(centerTrm);
            if (closest == null) return;
    
            if (closest.transform.TryGetComponent<Player.Player>(out var player))
            {
                player.GetModule<AgentHealth>().ApplyDamage(Damage);
            }
        }
    }
}