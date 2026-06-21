using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Shop
{
    // 상점에서 구매 가능한 패시브칩 전체 목록(랜덤 3개 추첨 풀).
    // id로 ShopChipDataSO를 역참조하는 용도로도 쓴다(보유 패시브 표시 시 아이콘/이름 조회).
    [CreateAssetMenu(menuName = "Shop/ShopChipCatalogSO", fileName = "ShopChipCatalog")]
    public class ShopChipCatalogSO : ScriptableObject
    {
        [Tooltip("상점 랜덤 추첨 풀(패시브칩). 무기칩은 인게임 드롭이라 여기 넣지 않는다.")]
        [SerializeField] private ShopChipDataSO[] passiveChips;

        private Dictionary<string, ShopChipDataSO> _byId;

        public IReadOnlyList<ShopChipDataSO> PassiveChips => passiveChips;

        public ShopChipDataSO GetById(string chipId)
        {
            if (_byId == null)
            {
                _byId = new Dictionary<string, ShopChipDataSO>();
                foreach (var c in passiveChips)
                    if (c != null) _byId[c.ChipId] = c;
            }
            return _byId.TryGetValue(chipId, out var so) ? so : null;
        }
    }
}
