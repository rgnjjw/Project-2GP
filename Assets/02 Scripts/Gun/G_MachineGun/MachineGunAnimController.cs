using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Gun.G_MachineGun
{
    public class MachineGunAnimController : GunAnimController
    {
        [SerializeField] private AnimParamSO bodyEquipAnimParam;
        [SerializeField] private AnimParamSO gunEquipAnimParam;

        [SerializeField] private AnimParamSO gunIdleAnimParam;
        [SerializeField] private AnimParamSO bodyIdleAnimParam;

        [SerializeField] private AnimParamSO gunRightFireAnimParam;
        [SerializeField] private AnimParamSO bodyRightFireAnimParam;

        [SerializeField] private AnimParamSO gunLeftFireAnimParam;
        [SerializeField] private AnimParamSO bodyLeftFireAnimParam;

        [SerializeField] private AnimParamSO skillBodyAnimParam;
        [SerializeField] private AnimParamSO skillGunAnimParam;

        private MachineGun _machineGun;

        protected override void Awake()
        {
            base.Awake();
            _machineGun = gun as MachineGun;
            if (_machineGun != null)
            {
                _machineGun.OnSkillStart += OnSkillStartAnim;
                _machineGun.OnSkillEnd += OnSkillEndAnim;
            }
        }

        private void OnDestroy()
        {
            if (_machineGun != null)
            {
                _machineGun.OnSkillStart -= OnSkillStartAnim;
                _machineGun.OnSkillEnd -= OnSkillEndAnim;
            }
        }

        protected override void OnEquipAnim()
        {
            gunRenderer.PlayClip(gunEquipAnimParam.ParamHash, 0, 0, 1);
            gunRenderer.PlayClip(bodyEquipAnimParam.ParamHash, 0, 0);
        }

        protected override void OnFireAnim()
        {
            AnimParamSO gunParam = _machineGun.LastFiredLeft ? gunLeftFireAnimParam : gunRightFireAnimParam;
            AnimParamSO bodyParam = _machineGun.LastFiredLeft ? bodyLeftFireAnimParam : bodyRightFireAnimParam;

            gunRenderer.PlayClip(gunParam.ParamHash, 0, 0, 1);
            gunRenderer.PlayClip(bodyParam.ParamHash, 0, 0);
        }

        private void OnSkillStartAnim()
        {
            if (skillBodyAnimParam != null)
                gunRenderer.PlayClip(skillBodyAnimParam.ParamHash, 0, 0);
            if (skillGunAnimParam != null)
                gunRenderer.PlayClip(skillGunAnimParam.ParamHash, 0, 0, 1);
        }

        private void OnSkillEndAnim()
        {
            gunRenderer.PlayClip(bodyIdleAnimParam.ParamHash, 0, 0);
            gunRenderer.PlayClip(gunIdleAnimParam.ParamHash, 0, 0,1);
        }
    }
}
