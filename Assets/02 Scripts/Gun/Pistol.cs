using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Core.AnimationSystem;
using _02_Scripts.Player;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class Pistol : Gun
    {
        [SerializeField] private AnimParamSO fireAnimParam;
        [SerializeField] private AnimParamSO equipAnimParam;
        [SerializeField] private AnimParamSO idleAnimParam;

        private float _nextFireTime;

        public override void Equip()
        {
            playerVisualController.ChangeVisual(PlayerVisualState.Pistol);
            playerVisualController.CurrentVisual.PlayClip(equipAnimParam.ParamHash, 0, 0);
        }

        public override void Fire()
        {
            if (Camera.main == null)
                return;

            if (Time.time < _nextFireTime)
                return;

            _nextFireTime = Time.time + fireDelay;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

            List<Vector3> points = GetReflectedPoints(ray, 1, 1000f);

            playerVisualController.CurrentVisual.PlayClip(fireAnimParam.ParamHash, 0, 0);

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