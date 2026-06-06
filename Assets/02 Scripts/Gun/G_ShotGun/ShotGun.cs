using System.Collections.Generic;
using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Gun.G_ShotGun
{
    public class ShotGun : Gun
    {
        [SerializeField] private int pelletCount = 8;     // 나가는 탄환 수
        [SerializeField] private float spreadAngle = 10f; //탄 퍼짐의 각도

        public override void Fire()
        {
            for (int i = 0; i < pelletCount; i++)
            {
                Ray ray = GetSpreadRay();
                List<Vector3> points = GetReflectedPoints(ray, 1, 1000f);
                
                base.Fire();
                
                trailRenderer.DrawTrail(points.ToArray());

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    if (hit.transform.TryGetComponent<AgentHealth>(out var health))
                        health.ApplyDamage(bulletDamage);
                }
            }
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

            Quaternion rotation = Quaternion.Euler(spread);
            Vector3 direction = rotation * baseRay.direction;

            return new Ray(baseRay.origin, direction);
        }
    }
}