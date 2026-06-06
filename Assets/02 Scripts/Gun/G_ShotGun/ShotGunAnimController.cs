using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Gun.G_ShotGun
{
    public class ShotGunAnimController : GunAnimController
    {
        [SerializeField] private AnimParamSO bodyEquipRight;
        [SerializeField] private AnimParamSO bodyEquipLeft;
        [SerializeField] private AnimParamSO gunEquip;
        
        [SerializeField] private AnimParamSO bodyFireRight;
        [SerializeField] private AnimParamSO bodyFireLeft;
        [SerializeField] private AnimParamSO gunFire;
        
        protected override void OnEquipAnim()
        {
            gunRenderer.PlayClip(bodyEquipLeft.ParamHash,0,0);
            gunRenderer.PlayClip(bodyEquipRight.ParamHash, 0, 0);
            gunRenderer.PlayClip(gunEquip.ParamHash, 0, 0);
        }

        protected override void OnFireAnim()
        {
            Debug.Log("dddddddddddddddddddddddddddddd");
            gunRenderer.PlayClip(bodyFireLeft.ParamHash, 0, 0);
            gunRenderer.PlayClip(bodyFireRight.ParamHash, 0, 0);
            gunRenderer.PlayClip(gunFire.ParamHash, 0, 0);
        }
    }
}