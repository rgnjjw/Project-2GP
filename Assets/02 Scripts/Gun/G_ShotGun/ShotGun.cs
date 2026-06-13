using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Gun.G_ShotGun
{
    public class ShotGun : Gun
    {
        [SerializeField] private int pelletCount = 8;
        [SerializeField] private float spreadAngle = 10f;

        public override void Fire()
        {
            if (Time.time < nextFireTime) return;
            nextFireTime = Time.time + fireDelay;

            for (int i = 0; i < pelletCount; i++)
            {
                Ray ray = GetSpreadRay();

                var tracer = Instantiate(tracerEffect, muzzleTrm.position, Quaternion.identity);
                tracer.AddPosition(muzzleTrm.position);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    hitEffect.transform.position = hit.point;
                    hitEffect.transform.forward = hit.normal;
                    hitEffect.Emit(1);

                    tracer.transform.position = hit.point;

                    if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                        enemy.GetModule<AgentHealth>().ApplyDamage(bulletDamage);
                }
                else
                {
                    tracer.transform.position = ray.origin + ray.direction * 1000f;
                }
            }

            base.Fire();
        }

        private Ray GetSpreadRay()
        {
            Camera cam = Camera.main;
            Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Ray baseRay = cam.ScreenPointToRay(center);

            Vector3 spread = new Vector3(
                Random.Range(-spreadAngle, spreadAngle),
                Random.Range(-spreadAngle, spreadAngle),
                0
            );

            Vector3 direction = Quaternion.Euler(spread) * baseRay.direction;
            return new Ray(baseRay.origin, direction);
        }
    }
}