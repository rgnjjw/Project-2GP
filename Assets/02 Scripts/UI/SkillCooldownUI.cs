using _02_Scripts.Manager;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    public class SkillCooldownUI : MonoBehaviour
    {
        [SerializeField] private GunManager gunManager;
        [SerializeField] private Image fillImage;
        [SerializeField] private float weaponSwitchSnapDuration = 0.15f;

        private Gun.Gun _currentGun;
        private Tween _snapTween;

        private void Awake()
        {
            gunManager.OnWeaponChanged += HandleWeaponChanged;
        }

        private void OnDestroy()
        {
            if (gunManager != null)
                gunManager.OnWeaponChanged -= HandleWeaponChanged;
            _snapTween?.Kill();
        }

        private void HandleWeaponChanged(Gun.Gun gun)
        {
            _currentGun = gun;

            float targetFill = _currentGun.SkillCooldownNormalized;
            _snapTween?.Kill();
            _snapTween = fillImage.DOFillAmount(targetFill, weaponSwitchSnapDuration).SetEase(Ease.OutQuad);
        }

        private void Update()
        {
            if (_currentGun == null) return;
            fillImage.fillAmount = _currentGun.SkillCooldownNormalized;
        }
    }
}