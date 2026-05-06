using UnityEngine;

namespace _02_Scripts.Weapon.Gun.FireAction
{
    public interface IFire
    {
        void Fire(Transform rayStartTransform,LayerMask layerMask, int bulletDamage);
    }
}