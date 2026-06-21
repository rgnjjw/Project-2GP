using _02_Scripts.Manager;
using UnityEngine;

namespace _02_Scripts.Chip.Weapon
{
    // 무기칩 효과: 장착 시 해당 무기를 해금하고, 레벨업 시 무기 스킬 레벨을 올린다.
    // (행동칩과 동일하게 ChipController가 장착/레벨업을 관리하고, 효과 적용은 GunManager로 위임)
    // [Chip] 어트리뷰트로 id를 고정하지 않는다 — WeaponChipDataSO이면 ChipInstance.GetEffect가
    // 직접 이 효과를 생성하므로, 무기 개수가 늘어도 코드 수정이 필요 없다.
    public class WeaponChip : IChip
    {
        private Player.Player _player;
        private int _weaponIndex;

        public void OnEquip(ChipInstance chip, Player.Player player)
        {
            if (chip.Data is not WeaponChipDataSO data) return;

            _player = player;
            _weaponIndex = data.WeaponIndex;

            GunManager gunManager = player.GetModule<GunManager>();
            if (gunManager == null)
            {
                Debug.LogError("[WeaponChip] GunManager 모듈을 찾을 수 없습니다.");
                return;
            }

            gunManager.UnlockWeapon(_weaponIndex);
            gunManager.SetWeaponSkillLevel(_weaponIndex, chip.CurrentLevel);
        }

        public void OnLevelUp(ChipInstance chip)
        {
            GunManager gunManager = _player != null ? _player.GetModule<GunManager>() : null;
            if (gunManager == null) return;

            gunManager.SetWeaponSkillLevel(_weaponIndex, chip.CurrentLevel);
        }

        public void OnUnequip(ChipInstance chip, Player.Player player)
        {
            // 무기는 한 번 해금되면 잠그지 않는다(행동칩과 달리 해제 개념이 없음).
        }
    }
}
