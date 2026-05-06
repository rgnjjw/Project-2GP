using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Weapon.Gun.FireAction
{
    public class HitScanFireAction : IFire
    {
        public void Fire(Transform rayStartTransform,LayerMask layerMask,int bulletDamage)
        {
            if (Physics.Raycast(rayStartTransform.position, rayStartTransform.forward, out RaycastHit hit,Mathf.Infinity,layerMask))
            {
                if (hit.transform.gameObject.TryGetComponent<AgentHealth>(out var agentHealth))
                {
                    agentHealth.ApplyDamage(bulletDamage);
                    Debug.Log("맞음");
                }
            }
        }
    }
}