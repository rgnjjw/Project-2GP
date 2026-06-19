using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Gun.G_Pistol
{
    public class PistolAnimController : GunAnimController
    {
        [SerializeField] private AnimParamSO equipAnimParam;
        [SerializeField] private AnimParamSO fireAnimParam;

        // 관통탄 스킬 발사 시 재생할 클립. 비워두면 일반 발사 모션(fireAnimParam)을 사용한다.
        [SerializeField] private AnimParamSO skillFireAnimParam;

        private Pistol _pistol;

        protected override void Awake()
        {
            base.Awake();
            _pistol = gun as Pistol;
            if (_pistol != null)
                _pistol.OnSkillStart += OnSkillFireAnim;
        }

        private void OnDestroy()
        {
            if (_pistol != null)
                _pistol.OnSkillStart -= OnSkillFireAnim;
        }

        protected override void OnEquipAnim()
        {
            gunRenderer.PlayClip(equipAnimParam.ParamHash, 0, 0);
        }

        protected override void OnFireAnim()
        {
            gunRenderer.PlayClip(fireAnimParam.ParamHash, 0, 0);
        }

        private void OnSkillFireAnim()
        {
            AnimParamSO param = skillFireAnimParam != null ? skillFireAnimParam : fireAnimParam;
            gunRenderer.PlayClip(param.ParamHash, 0, 0);
        }
    }
}
