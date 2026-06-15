using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "JumpAttackSkillSO", menuName = "Skill/JumpAttackSkillSO", order = 0)]
    public class JumpAttackSkillSO : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float JumpHeight { get; private set; } = 5f;
        [field: SerializeField] public float JumpDuration { get; private set; } = 0.8f;
        [field: SerializeField] public AnimParamSO JumpAnimParam { get; private set; }
        [field: SerializeField] public AnimParamSO AirAnimParam { get; private set; }
        [field: SerializeField] public AnimParamSO LandingAnimParam { get; private set; }

        public override void ExecuteSkill(Transform centerTrm)
        {
            var enemy = centerTrm.GetComponent<Enemy>();
            if (enemy == null) return;

            var target = DamageAreaDetection?.GetClosest(centerTrm);
            if (target == null) return;

            var behaviour = enemy.gameObject.AddComponent<JumpAttackBehaviour>();
            behaviour.Execute(enemy, target.transform.position, this, NotifyComplete);
        }
    }
}
