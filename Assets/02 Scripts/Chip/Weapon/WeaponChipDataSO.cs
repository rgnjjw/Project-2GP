using UnityEngine;

namespace _02_Scripts.Chip.Weapon
{
    // 무기칩도 행동칩과 동일하게 Chip/ 시스템(ChipDataSO)으로 다룬다.
    // → 인게임 카드(ChipCardUI)에서 드롭/장착/레벨업이 그대로 동작한다.
    // WeaponIndex는 GunManager.weapons 배열의 인덱스(0=권총,1=샷건,2=머신건 등).
    [CreateAssetMenu(fileName = "WeaponChipDataSO", menuName = "Chip/WeaponChipDataSO")]
    public class WeaponChipDataSO : ChipDataSO
    {
        [field: SerializeField] public int WeaponIndex { get; private set; }

        // 무기칩은 데이터 기반이라 id 등록(ChipFactory) 없이 전용 효과를 직접 생성한다.
        public override IChip CreateEffect() => new WeaponChip();
    }
}
