using System;
using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Chip;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Core.Utility;
using _02_Scripts.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class Gun : MonoBehaviour
    {
        [field: SerializeField] public AgentRenderer Renderer { get;private set; }
        [SerializeField] protected GunTrailRenderer trailRenderer;
        [SerializeField] protected LayerMask layerMask;
        [SerializeField] protected int bulletDamage;
        [SerializeField] protected float fireDelay = 0.2f;
        [SerializeField] protected Transform muzzleTrm;
        [SerializeField] private RecoilEvent recoilEvent;
        [SerializeField] private string gunFireEffect;//이펙트
        public event Action OnFire;
        public event Action OnEquip;
        
        protected float nextFireTime;

        public virtual void Equip()
        {
            OnEquip?.Invoke();
        }


        public virtual void Fire()
        {
            EventBus.Publish(recoilEvent);
            EventBus.Publish(new EffectEvent(gunFireEffect));
            OnFire?.Invoke();
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