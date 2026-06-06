using System;
using _02_Scripts.Agent;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public abstract class GunAnimController : MonoBehaviour
    {
        [SerializeField] protected Gun gun;
        protected AgentRenderer gunRenderer;
        private void Awake()
        {
            gunRenderer = gun.Renderer;
            gun.OnEquip += OnEquipAnim;
            gun.OnFire += OnFireAnim;
        }

        protected abstract void OnEquipAnim();
        protected abstract void OnFireAnim();

    }
}