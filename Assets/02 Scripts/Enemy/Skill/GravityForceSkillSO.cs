using System.Collections;
using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    [CreateAssetMenu(fileName = "GravityForceSkillSO", menuName = "Skill/GravityForceSkillSO", order = 0)]
    public class GravityForceSkillSO : SkillSO
    {
        [Header("끌어당기기 / 밀어내기")]
        [SerializeField] private float forceDuration = 1.5f;
        [Tooltip("양수면 끌어당김(적 쪽으로), 음수면 밀어냄(적 반대쪽으로)")]
        [SerializeField] private float forceSpeed = 8f;
        [Tooltip("끌어당기기일 때만 적용: 이 거리보다 가까워지면 즉시 종료하고 폭발")]
        [SerializeField] private float minDistance = 1.5f;

        [Header("종료 시 폭발")]
        [SerializeField] private int explosionDamage = 25;

        private EnemyAnimationEvent _animEvent;
        private EnemyVfxController _vfx;
        private Enemy _enemy;
        private Transform _target;

        public override void ExecuteSkill(Enemy enemy)
        {
            _enemy = enemy;
            _target = TargetFinder?.GetClosest(enemy.transform) ?? enemy.CurrentTarget;
            _animEvent = enemy.GetModule<EnemyAnimationEvent>();
            _vfx = enemy.GetModule<EnemyVfxController>();

            _animEvent.OnAttack += HandleAttack;
        }

        private void HandleAttack()
        {
            _animEvent.OnAttack -= HandleAttack;

            if (_target == null)
            {
                NotifyComplete();
                return;
            }

            _enemy.StartCoroutine(ForceRoutine());
        }

        private IEnumerator ForceRoutine()
        {
            var player = _target.GetComponent<Player.Player>();
            var mover = player?.GetModule<AgentMover>();

            if (player == null || mover == null)
            {
                NotifyComplete();
                yield break;
            }

            bool isPulling = forceSpeed > 0f;

            _vfx?.Play(EnemyVfxType.GravityZone); 
            mover.BeginSlide(Vector3.zero);

            float elapsed = 0f;

            while (elapsed < forceDuration)
            {
                elapsed += Time.fixedDeltaTime;

                Vector3 toEnemy = _enemy.transform.position - player.transform.position;
                toEnemy.y = 0f; // 수평으로만 작용 (공중으로 띄우지 않음)

                float distance = toEnemy.magnitude;

                if (isPulling && distance <= minDistance)
                    break;

                Vector3 dir = distance > 0.0001f ? toEnemy.normalized : Vector3.zero;
                Vector3 forceVelocity = dir * forceSpeed;
                mover.UpdateSlideVelocity(forceVelocity);

                yield return new WaitForFixedUpdate();
            }

            mover.EndSlide();

            Explode(player);
            NotifyComplete();
        }

        private void Explode(Player.Player player)
        {
            if (DamageAreaDetection != null)
            {
                foreach (var t in DamageAreaDetection.GetAllInRange(_enemy.transform))
                {
                    if (t.TryGetComponent<Player.Player>(out var p))
                        p.GetModule<AgentHealth>().ApplyDamage(explosionDamage);
                }
            }
            else if (player != null)
            {
                player.GetModule<AgentHealth>().ApplyDamage(explosionDamage);
            }

            _vfx?.Play(EnemyVfxType.Explosion);
        }
    }
}