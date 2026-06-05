using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public abstract class Gun : MonoBehaviour, IModule
    {
        [SerializeField] protected GunTrailRenderer trailRenderer;
        [SerializeField] protected LayerMask layerMask;
        [SerializeField] protected int bulletDamage;
        [SerializeField] protected float fireDelay = 0.2f;
        
        protected float nextFireTime;
        protected PlayerVisualController playerVisualController;

        public void Initialize(ModuleOwner owner)
        {
            playerVisualController = owner.GetModule<PlayerVisualController>();
        }

        public abstract void Equip();

        public abstract void Fire();

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