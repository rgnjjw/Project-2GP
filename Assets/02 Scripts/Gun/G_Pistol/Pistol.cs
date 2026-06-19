using System;
using _02_Scripts.Agent;
using _02_Scripts.Core.Utility;
using _02_Scripts.Gun.Skill;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class Pistol : Gun
    {
        [SerializeField] private PistolSkillDataSO skillData;
        [SerializeField] private GameObject piercingBulletPrefab;

        [Header("Skill Feedback")]
        [SerializeField] private ParticleSystem skillFireEffect;
        [SerializeField] private RecoilEvent skillRecoilEvent;

        private int _skillLevel = 1;
        private float _skillCooldownRemaining;

        public float SkillCooldownRemaining => _skillCooldownRemaining;
        public bool IsSkillReady => _skillCooldownRemaining <= 0f;

        public override void SetSkillLevel(int level) => _skillLevel = level;

        public override void OnSkillPressed()
        {
            if (skillData == null || !IsSkillReady) return;
            if (piercingBulletPrefab == null || Camera.main == null) return;
            
            var data = skillData.GetLevel(_skillLevel);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
            
            GameObject bulletObj = Instantiate(piercingBulletPrefab, muzzleTrm.position, Quaternion.identity);
            var bullet = bulletObj.GetComponent<PiercingBullet>();
            bullet.Initialize(ray.direction, data.Damage, data.BulletSpeed, data.AutoDeleteTime, skillData.HitMask);

            _skillCooldownRemaining = data.Cooldown;

            // 스킬 전용 파티클 + 반동 (인스펙터에서 일반 발사와 따로 설정)
            skillFireEffect?.Play();
            EventBus.Publish(skillRecoilEvent);

            // 스킬 발사 애니메이션 트리거 (PistolAnimController가 구독)
            FireSkillStart();
        }

        public override void TickSkill(float deltaTime)
        {
            if (_skillCooldownRemaining > 0f)
                _skillCooldownRemaining -= deltaTime;
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