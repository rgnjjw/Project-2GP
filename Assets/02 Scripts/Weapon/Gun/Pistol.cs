using System.Collections.Generic;
using UnityEngine;
using _02_Scripts.Agent;
using _02_Scripts.Core.AnimationSystem;
using _02_Scripts.Player;

namespace _02_Scripts.Weapon.Gun
{
    public class Pistol : Gun
    {
        [SerializeField] private AnimParamSO fireAnimParam;
        [SerializeField] private AnimParamSO equipAnimParam;
        
        public override void Equip()
        {
            playerVisualController.ChangeVisual(PlayerVisualState.GUN);
            playerVisualController.CurrentVisual.PlayClip(equipAnimParam.ParamHash,0,0);
        }

        public override void Fire()
        {
            if (Camera.main == null) return;
            
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            List<Vector3> points = GetReflectedPoints(ray, 1, 1000f);
            
            playerVisualController.CurrentVisual.PlayClip(fireAnimParam.ParamHash,0,0);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.TryGetComponent<AgentHealth>(out var agentHealth))
                {
                    agentHealth.ApplyDamage(bulletDamage);
                }
            }

            trailRenderer.DrawTrail(points.ToArray());
        }
    }
}