using System;
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
        [SerializeField] protected TrailRenderer tracerEffect;

        public virtual bool IsAutoFire => false;
        public bool isFiring = false;
        public event Action OnFire;
        public event Action OnEquip;
        public event Action OnSkillStart;
        public event Action OnSkillEnd;

        protected float nextFireTime;

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

        protected void FireSkillStart() => OnSkillStart?.Invoke();
        protected void FireSkillEnd() => OnSkillEnd?.Invoke();

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
