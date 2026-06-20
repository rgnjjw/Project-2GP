using System;
using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Core.Utility;
using _02_Scripts.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class Gun : MonoBehaviour
    {
        [field: SerializeField] public AgentRenderer Renderer { get; private set; }
        [SerializeField] protected LayerMask layerMask;
        [SerializeField] protected int bulletDamage;
        [SerializeField] protected float fireDelay = 0.2f;
        [SerializeField] protected Transform muzzleTrm;
        [SerializeField] private RecoilEvent recoilEvent;
        [SerializeField] protected ParticleSystem fireEffect;
        [SerializeField] protected ParticleSystem hitEffect;
        [SerializeField] protected float beamDuration = 0.03f;

        private readonly Dictionary<LineRenderer, Coroutine> _beamRoutines = new();

        public virtual bool IsAutoFire => false;
        public bool isFiring = false;
        public event Action OnFire;
        public event Action OnEquip;
        public event Action OnSkillStart;
        public event Action OnSkillEnd;

        protected float nextFireTime;

        // Camera.main은 내부적으로 태그 검색을 수행하므로 매 발사(샷건은 펠릿마다)마다 호출하면 비싸다. 캐싱한다.
        private Camera _cachedCamera;
        protected Camera MainCamera
        {
            get
            {
                if (_cachedCamera == null) _cachedCamera = Camera.main;
                return _cachedCamera;
            }
        }

        public virtual void Equip()
        {
            OnEquip?.Invoke();
        }

        public virtual void Fire()
        {
            EventBus.Publish(recoilEvent);
            PlayFireEffect();
            OnFire?.Invoke();
        }

        protected virtual void PlayFireEffect()
        {
            fireEffect?.Play();
        }

        protected void PlayFireFeedbackOnly()
        {
            EventBus.Publish(recoilEvent);
            PlayFireEffect();
        }

        protected void DealDamage(AgentHealth target, int damage)
        {
            if (target == null) return;
            target.ApplyDamage(damage);
            EventBus.Publish(new PlayerDamageDealtEvent(damage));
        }

        protected Vector3 GetHitPoint(Ray ray, float maxDistance)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
                return hit.point;
            return ray.origin + ray.direction * maxDistance;
        }

        public virtual void OnSkillPressed() { }
        public virtual void OnSkillReleased() { }
        public virtual void TickSkill(float deltaTime) { }
        public virtual void SetSkillLevel(int level) { }

        public virtual float SkillCooldownRemaining => 0f;
        public virtual float SkillCooldownMax => 0f;
        public float SkillCooldownNormalized => SkillCooldownMax <= 0f ? 1f : 1f - (SkillCooldownRemaining / SkillCooldownMax);

        protected void FireSkillStart() => OnSkillStart?.Invoke();
        protected void FireSkillEnd() => OnSkillEnd?.Invoke();

        // 미리 달아둔 LineRenderer를 재사용: start(총구)→end(명중점)로 위치만 갱신하고 잠깐 켰다가 끈다.
        protected void ShowBeam(LineRenderer beam, Vector3 start, Vector3 end)
        {
            if (beam == null) return;

            beam.useWorldSpace = true;
            beam.positionCount = 2;
            beam.SetPosition(0, start);
            beam.SetPosition(1, end);
            beam.enabled = true;

            if (_beamRoutines.TryGetValue(beam, out var running) && running != null)
                StopCoroutine(running);
            _beamRoutines[beam] = StartCoroutine(HideBeamAfter(beam, beamDuration));
        }

        private IEnumerator HideBeamAfter(LineRenderer beam, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (beam != null) beam.enabled = false;
            _beamRoutines[beam] = null;
        }

        protected List<Vector3> GetReflectedPoints(Ray ray, int maxBounce, float maxDistance)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(ray.origin);

            for (int i = 0; i < maxBounce; i++)
            {
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
                {
                    points.Add(hit.point);
                    ray = new Ray(hit.point + hit.normal * 0.001f, Vector3.Reflect(ray.direction, hit.normal));
                }
                else
                {
                    points.Add(ray.origin + ray.direction * maxDistance);
                    break;
                }
            }

            return points;
        }
    }
}
