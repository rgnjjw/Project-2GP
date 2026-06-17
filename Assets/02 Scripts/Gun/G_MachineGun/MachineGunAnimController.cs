using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Gun.G_MachineGun
{
    public class MachineGunAnimController : GunAnimController
    {
        [SerializeField] private AnimParamSO bodyEquipAnimParam;
        [SerializeField] private AnimParamSO gunEquipAnimParam;
        
        [SerializeField] private AnimParamSO gunRightFireAnimParam;
        [SerializeField] private AnimParamSO bodyRightFireAnimParam;
        
        [SerializeField] private AnimParamSO gunLeftFireAnimParam;
        [SerializeField] private AnimParamSO bodyLeftFireAnimParam;

        private MachineGun _machineGun;

        protected override void Awake()
        {
            base.Awake();
            _machineGun = gun as MachineGun;
        }

        protected override void OnEquipAnim()
        {
            gunRenderer.PlayClip(gunEquipAnimParam.ParamHash, 0,0, 1);
            gunRenderer.PlayClip(bodyEquipAnimParam.ParamHash, 0, 0);
        }

        protected override void OnFireAnim()
        {
            AnimParamSO gunParam = _machineGun.LastFiredLeft ? gunLeftFireAnimParam : gunRightFireAnimParam;
            AnimParamSO bodyParam = _machineGun.LastFiredLeft ? bodyLeftFireAnimParam : bodyRightFireAnimParam;
            
            Debug.Log(gunParam);
            Debug.Log(bodyParam);
            
            gunRenderer.PlayClip(gunParam.ParamHash, 0, 0,1);
            gunRenderer.PlayClip(bodyParam.ParamHash, 0, 0);
        }
    }
}
