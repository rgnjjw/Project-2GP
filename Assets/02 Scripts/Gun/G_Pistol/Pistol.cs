using _02_Scripts.Agent;
using _02_Scripts.Gun.Skill;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class Pistol : Gun
    {
        [SerializeField] private PistolSkillDataSO skillData;
        [SerializeField] private GameObject ricochetBulletPrefab;

        private int _skillLevel = 1;
        private float _skillCooldownRemaining;
        private bool _isCharging;

        public bool IsCharging => _isCharging;
        public float SkillCooldownRemaining => _skillCooldownRemaining;
        public bool IsSkillReady => _skillCooldownRemaining <= 0f;

        public override void SetSkillLevel(int level) => _skillLevel = level;

        public override void OnSkillPressed()
        {
            if (skillData == null || !IsSkillReady) return;
            _isCharging = true;
            FireSkillStart();
        }

        public override void OnSkillReleased()
        {
            if (!_isCharging) return;
            _isCharging = false;
            FireSkillEnd();
            FireRicochetBullet();
        }

        public override void TickSkill(float deltaTime)
        {
            if (_skillCooldownRemaining > 0f)
                _skillCooldownRemaining -= deltaTime;
        }

        private void FireRicochetBullet()
        {
            if (skillData == null || Camera.main == null) return;

            var data = skillData.GetLevel(_skillLevel);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

            GameObject bulletObj = Instantiate(ricochetBulletPrefab, muzzleTrm.position, Quaternion.identity);
            var bullet = bulletObj.GetComponent<RicochetBullet>();
            bullet.Initialize(ray.direction, data.Damage, data.BulletSpeed, data.AutoDeleteTime, skillData.HitMask);

            _skillCooldownRemaining = data.Cooldown;
        }

        public override void Fire()
        {
            if (Camera.main == null) return;
            if (Time.time < nextFireTime) return;

            nextFireTime = Time.time + fireDelay;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

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

            base.Fire();
        }
    }
}