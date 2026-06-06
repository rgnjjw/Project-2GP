using System.Collections.Generic;
using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class Pistol : Gun
    {
        public override void Fire()
        {
            if (Camera.main == null)
                return;

            if (Time.time < nextFireTime)
                return;

            nextFireTime = Time.time + fireDelay;
            
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

            List<Vector3> points = GetReflectedPoints(ray, 1, 1000f);

            base.Fire();

            trailRenderer.DrawTrail(points.ToArray());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.TryGetComponent(out AgentHealth agentHealth))
                {
                    agentHealth.ApplyDamage(bulletDamage);
                }
            }
        }
    }
}