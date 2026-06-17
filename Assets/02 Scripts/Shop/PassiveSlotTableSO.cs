using UnityEngine;

namespace _02_Scripts.Shop
{
    [CreateAssetMenu(menuName = "Shop/PassiveSlotTableSO", fileName = "PassiveSlotTable")]
    public class PassiveSlotTableSO : ScriptableObject
    {
        [SerializeField] private int[] _slotCountPerLevel;
        [SerializeField] private int _defaultSlotCount = 1;

        public int GetSlotCount(int playerLevel)
        {
            int idx = playerLevel - 1;
            if (_slotCountPerLevel == null || idx < 0 || idx >= _slotCountPerLevel.Length)
                return _defaultSlotCount;
            return _slotCountPerLevel[idx];
        }
    }
}
