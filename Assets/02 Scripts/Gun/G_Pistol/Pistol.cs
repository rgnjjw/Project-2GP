using System;
using System.Collections.Generic;
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

        [Header("Beam (LineRenderer)")]
        [SerializeField] private LineRenderer fireBeam;
        [SerializeField] private LineRenderer skillBeam;

        [Header("Skill Feedback")]
        [SerializeField] private ParticleSystem skillFireEffect;
        [SerializeField] private RecoilEvent skillRecoilEvent;

        private int _skillLevel = 1;
        private float _skillCooldownRemaining;
        private float _skillCooldownMax;
        private readonly HashSet<Enemy.Enemy> _skillHitEnemies = new();

        public override float SkillCooldownRemaining => _skillCooldownRemaining;
        public override float SkillCooldownMax => _skillCooldownMax;
        public bool IsSkillReady => _skillCooldownRemaining <= 0f;

        public override void SetSkillLevel(int level) => _skillLevel = level;

        public override void OnSkillPressed()
        {
            if (skillData == null || !IsSkillReady) return;
            if (Camera.main == null) return;

            var data = skillData.GetLevel(_skillLevel);
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

            Vector3 endPoint = ray.origin + ray.direction * 1000f;
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, skillData.HitMask);
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                {
                    if (_skillHitEnemies.Add(enemy))
                        DealDamage(enemy.GetModule<AgentHealth>(), data.Damage);
                }
                else
                {
                    endPoint = hit.point;
                    break;
                }
            }
            _skillHitEnemies.Clear();

            ShowBeam(skillBeam, muzzleTrm.position, endPoint);

            _skillCooldownMax = data.Cooldown;
            _skillCooldownRemaining = data.Cooldown;

            skillFireEffect?.Play();
            EventBus.Publish(skillRecoilEvent);

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

            Vector3 endPoint = ray.origin + ray.direction * 1000f;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                endPoint = hit.point;

                hitEffect.transform.position = hit.point;
                hitEffect.transform.forward = hit.normal;
                hitEffect.Emit(1);

                if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                    DealDamage(enemy.GetModule<AgentHealth>(), bulletDamage);
            }

            ShowBeam(fireBeam, muzzleTrm.position, endPoint);

            base.Fire();
        }
    }
}