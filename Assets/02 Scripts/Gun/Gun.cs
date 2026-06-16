using System;
using System.Collections.Generic;
using _02_Scripts.Agent;
using Unity.Cinemachine;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class Gun : MonoBehaviour
    {
        [field: SerializeField] public AgentRenderer Renderer { get; private set; }
        [SerializeField] protected CinemachineImpulseSource impulseSource;
        [SerializeField] protected RecoilDataSO recoilData;
        [SerializeField] protected LayerMask layerMask;
        [SerializeField] protected int bulletDamage;
        [SerializeField] protected float fireDelay = 0.2f;
        [SerializeField] protected Transform muzzleTrm;
        [SerializeField] protected ParticleSystem fireEffect;
        [SerializeField] protected ParticleSystem hitEffect;
        [SerializeField] protected TrailRenderer tracerEffect;

        public bool isFiring = false;
        public event Action OnFire;
        public event Action OnEquip;

        protected float nextFireTime;

        public virtual void Equip()
        {
            if (recoilData != null && impulseSource != null)
            {
                impulseSource.ImpulseDefinition.ImpulseShape = recoilData.ImpulseShape;
                impulseSource.ImpulseDefinition.ImpulseDuration = recoilData.Duration;
            }

            OnEquip?.Invoke();
        }

        public virtual void Fire()
        {
            fireEffect.Play();

            if (impulseSource != null && recoilData != null)
                impulseSource.GenerateImpulse(recoilData.Direction * recoilData.Force);

            OnFire?.Invoke();
        }

        protected Vector3 GetHitPoint(Ray ray, float maxDistance)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
                return hit.point;
            return ray.origin + ray.direction * maxDistance;
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