using _02_Scripts.Agent;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public abstract class GunAnimController : MonoBehaviour,IModule
    {
        protected AgentRenderer gunRenderer;
        public void Initialize(ModuleOwner owner)
        {
            if (owner is Gun gun)
            {
                gunRenderer = gun.Renderer;
                gun.OnEquip += OnEquipAnim;
                gun.OnFire += OnFireAnim;
            }
        }

        protected abstract void OnEquipAnim();
        protected abstract void OnFireAnim();

    }
}