using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Gun.G_ShotGun
{
    public class ShotGunAnimController : GunAnimController
    {
        [SerializeField] private AnimParamSO bodyEquipRight;
        [SerializeField] private AnimParamSO bodyEquipLeft;
        [SerializeField] private AnimParamSO gunEquip;

        [SerializeField] private AnimParamSO gunEnd;
        [SerializeField] private AnimParamSO bodyEndLeft;
        [SerializeField] private AnimParamSO bodyEndRight;

        [SerializeField] private AnimParamSO bodyFireRight;
        [SerializeField] private AnimParamSO bodyFireLeft;
        [SerializeField] private AnimParamSO gunFire;

        [SerializeField] private AnimParamSO skillBodyAnimParam;
        [SerializeField] private AnimParamSO skillGunAnimParam;

        private ShotGun _shotGun;

        protected override void Awake()
        {
            base.Awake();
            _shotGun = gun as ShotGun;
            if (_shotGun != null)
            {
                _shotGun.OnSkillStart += OnSkillStartAnim;
                _shotGun.OnSkillEnd += OnSkillEndAnim;
            }
        }

        private void OnDestroy()
        {
            if (_shotGun != null)
            {
                _shotGun.OnSkillStart -= OnSkillStartAnim;
                _shotGun.OnSkillEnd -= OnSkillEndAnim;
            }
        }

        protected override void OnEquipAnim()
        {
            gunRenderer.PlayClip(bodyEquipLeft.ParamHash, 0, 0, 2);
            gunRenderer.PlayClip(bodyEquipRight.ParamHash, 0, 0, 3);
            gunRenderer.PlayClip(gunEquip.ParamHash, 0, 0, 1);
        }

        protected override void OnFireAnim()
        {
            gunRenderer.PlayClip(bodyFireLeft.ParamHash, 0, 0, 2);
            gunRenderer.PlayClip(bodyFireRight.ParamHash, 0, 0, 3);
            gunRenderer.PlayClip(gunFire.ParamHash, 0, 0, 1);
        }

        private void OnSkillStartAnim()
        {
            if (skillBodyAnimParam != null)
                gunRenderer.PlayClip(skillBodyAnimParam.ParamHash, 0,0, 3);
            if (skillGunAnimParam != null)
                gunRenderer.PlayClip(skillGunAnimParam.ParamHash, 0, 0, 1);
        }

        private void OnSkillEndAnim()
        {
            gunRenderer.PlayClip(gunEnd.ParamHash, 0, 0,1);
            gunRenderer.PlayClip(bodyEndRight.ParamHash, 0, 0, 3);
            gunRenderer.PlayClip(bodyEndLeft.ParamHash, 0, 0, 2);
        }
    }
}