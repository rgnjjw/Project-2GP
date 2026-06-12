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
            Vector3 hitPoint = GetHitPoint(ray, 1000f);

            base.Fire();

            SpawnBulletTrail(muzzleTrm.position, hitPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                    enemy.GetModule<AgentHealth>().ApplyDamage(bulletDamage);
            }
        }
    }
}