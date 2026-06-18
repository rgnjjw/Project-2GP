using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class PistolAnimController : GunAnimController
    {
        [SerializeField] private AnimParamSO equipAnimParam;
        [SerializeField] private AnimParamSO fireAnimParam;

        [Header("Skill")]
        [SerializeField] private AnimParamSO skillChargeBodyAnimParam;
        [SerializeField] private AnimParamSO skillChargeGunAnimParam;
        [SerializeField] private AnimParamSO skillFireBodyAnimParam;
        [SerializeField] private AnimParamSO skillFireGunAnimParam;

        private Pistol _pistol;

        protected override void Awake()
        {
            base.Awake();
            _pistol = gun as Pistol;
            if (_pistol != null)
            {
                _pistol.OnSkillStart += OnSkillChargeAnim;
                _pistol.OnSkillEnd += OnSkillFireAnim;
            }
        }

        private void OnDestroy()
        {
            if (_pistol != null)
            {
                _pistol.OnSkillStart -= OnSkillChargeAnim;
                _pistol.OnSkillEnd -= OnSkillFireAnim;
            }
        }

        protected override void OnEquipAnim()
        {
            gunRenderer.PlayClip(equipAnimParam.ParamHash, 0, 0);
        }

        protected override void OnFireAnim()
        {
            gunRenderer.PlayClip(fireAnimParam.ParamHash, 0, 0);
        }

        private void OnSkillChargeAnim()
        {
            if (skillChargeBodyAnimParam != null)
                gunRenderer.PlayClip(skillChargeBodyAnimParam.ParamHash, 0, 0);
            if (skillChargeGunAnimParam != null)
                gunRenderer.PlayClip(skillChargeGunAnimParam.ParamHash, 0, 0, 1);
        }

        private void OnSkillFireAnim()
        {
            if (skillFireBodyAnimParam != null)
                gunRenderer.PlayClip(skillFireBodyAnimParam.ParamHash, 0, 0);
            if (skillFireGunAnimParam != null)
                gunRenderer.PlayClip(skillFireGunAnimParam.ParamHash, 0, 0, 1);
        }
    }
}