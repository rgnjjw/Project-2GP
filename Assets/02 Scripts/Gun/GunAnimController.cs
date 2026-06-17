using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public abstract class GunAnimController : MonoBehaviour
    {
        [SerializeField] protected Gun gun;
        protected AgentRenderer gunRenderer;

        protected virtual void Awake()
        {
            gunRenderer = gun.Renderer;
            gun.OnEquip += OnEquipAnim;
            gun.OnFire += OnFireAnim;
        }

        protected abstract void OnEquipAnim();
        protected abstract void OnFireAnim();
    }
}
