using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class PistolAnimController : GunAnimController
    {
        [SerializeField] private AnimParamSO equipAnimParam;
        [SerializeField] private AnimParamSO fireAnimParam;
        protected override void OnEquipAnim()
        {
            gunRenderer.PlayClip(equipAnimParam.ParamHash,0,0);
        }

        protected override void OnFireAnim()
        {
            gunRenderer.PlayClip(fireAnimParam.ParamHash, 0, 0);
        }
    }
}