using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class Pistol : Gun
    {
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