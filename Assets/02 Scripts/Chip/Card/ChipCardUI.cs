using System;
using UnityEngine;

namespace _02_Scripts.Chip.Card
{
    [Serializable]
    public class StageCardChipData //스테이지에 등장 가능한 칩 능력 카드 종류
    {
        public ChipDataSO ChipData;
    }

    public class ChipCardUI : MonoBehaviour
    {
        [SerializeField] private ChipCardButtonUI[] chipCardButtonUI;
        
    }
}