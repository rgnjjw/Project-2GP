using System.Collections;
using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "SelfDestructSkillSO", menuName = "Skill/SelfDestructSkillSO", order = 0)]
    public class SelfDestructSkillSO : SkillSO
    {
        [field: SerializeField] public int Damage { get; private set; } = 40;

        [Header("자폭 타이밍")]
        [Tooltip("폭발(OnAttack) 이후, 실제로 죽기까지 추가 지연 (이펙트 보여줄 시간)")]
        [SerializeField] private float deathDelay = 0.1f;

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

            Explode();
        }

        private void Explode()
        {
            if (DamageAreaDetection != null)
            {
                foreach (var t in DamageAreaDetection.GetAllInRange(_enemy.transform))
                {
                    if (t.TryGetComponent<Player.Player>(out var player))
                        player.GetModule<AgentHealth>().ApplyDamage(Damage);
                }
            }

            _vfx?.Play(EnemyVfxType.Explosion);

            _enemy.StartCoroutine(KillSelfAfterDelay());
        }

        private IEnumerator KillSelfAfterDelay()
        {
            NotifyComplete();

            if (deathDelay > 0f)
                yield return new WaitForSeconds(deathDelay);

            var health = _enemy.GetModule<AgentHealth>();
            if (health != null)
                health.ApplyDamage(int.MaxValue);
        }
    }
}