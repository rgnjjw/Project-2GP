using _02_Scripts.Agent;
using _02_Scripts.Gun.Skill;
using UnityEngine;

namespace _02_Scripts.Gun.G_ShotGun
{
    public class ShotGun : Gun
    {
        [SerializeField] private int pelletCount = 8;
        [SerializeField] private float spreadAngle = 10f;
        [SerializeField] private ShotGunSkillDataSO skillData;
        [SerializeField] private Transform chainsawOrigin;

        private const float SkillTickInterval = 0.1f;

        private int _skillLevel = 1;
        private float _skillCooldownRemaining;
        private bool _isGrinding;
        private float _tickTimer;

        public bool IsGrinding => _isGrinding;
        public bool IsSkillReady => _skillCooldownRemaining <= 0f;

        public override void SetSkillLevel(int level) => _skillLevel = level;

        public override void OnSkillPressed()
        {
            if (skillData == null || !IsSkillReady) return;
            _isGrinding = true;
            _tickTimer = 0f;
            FireSkillStart();
        }

        public override void OnSkillReleased()
        {
            if (!_isGrinding) return;
            _isGrinding = false;
            FireSkillEnd();
            if (skillData != null)
                _skillCooldownRemaining = skillData.GetLevel(_skillLevel).Cooldown;
        }

        public override void TickSkill(float deltaTime)
        {
            if (_skillCooldownRemaining > 0f)
                _skillCooldownRemaining -= deltaTime;

            if (!_isGrinding || skillData == null) return;

            _tickTimer += deltaTime;

            if (_tickTimer >= SkillTickInterval)
            {
                _tickTimer -= SkillTickInterval;

                var data = skillData.GetLevel(_skillLevel);
                int dmg = Mathf.RoundToInt(data.DamagePerSecond * SkillTickInterval);
                if (dmg <= 0) dmg = 1;

                Transform origin = chainsawOrigin != null ? chainsawOrigin : muzzleTrm;
                Collider[] hits = Physics.OverlapSphere(origin.position, data.Range, skillData.EnemyMask);

                AgentHealth closest = null;
                float closestDist = float.MaxValue;
                foreach (var col in hits)
                {
                    if (!col.transform.TryGetComponent<Enemy.Enemy>(out var enemy)) continue;
                    float dist = Vector3.Distance(origin.position, col.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = enemy.GetModule<AgentHealth>();
                    }
                }
                if (closest != null)
                    DealDamage(closest, dmg);
            }
        }

        public override void Fire()
        {
            if (Time.time < nextFireTime) return;
            nextFireTime = Time.time + fireDelay;

            for (int i = 0; i < pelletCount; i++)
            {
                Ray ray = GetSpreadRay();

                var tracer = Instantiate(tracerEffect, muzzleTrm.position, Quaternion.identity);
                tracer.AddPosition(muzzleTrm.position);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    hitEffect.transform.position = hit.point;
                    hitEffect.transform.forward = hit.normal;
                    hitEffect.Emit(1);

                    tracer.transform.position = hit.point;

                    if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                        DealDamage(enemy.GetModule<AgentHealth>(), bulletDamage);
                }
                else
                {
                    tracer.transform.position = ray.origin + ray.direction * 1000f;
                }
            }

            base.Fire();
        }

        private Ray GetSpreadRay()
        {
            Camera cam = Camera.main;
            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Ray baseRay = cam.ScreenPointToRay(center);

            Vector3 spread = new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            );

            Vector3 direction = Quaternion.Euler(spread) * baseRay.direction;
            return new Ray(baseRay.origin, direction);
        }
    }
}