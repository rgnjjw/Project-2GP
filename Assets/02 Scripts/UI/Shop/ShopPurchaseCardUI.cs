using System;
using _02_Scripts.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI.Shop
{
    // 상점 상단의 "랜덤 패시브 구매" 카드 한 장.
    public class ShopPurchaseCardUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button button;
        [SerializeField] private GameObject soldOverlay; // 구매 완료 표시(선택)

        private ShopChipDataSO _chip;
        private Action<ShopChipDataSO> _onBuy;

        // chip이 null이면 빈 슬롯(구매 완료 등)으로 비활성 표시한다.
        public void Setup(ShopChipDataSO chip, Action<ShopChipDataSO> onBuy)
        {
            _chip = chip;
            _onBuy = onBuy;

            bool hasChip = chip != null;
            if (soldOverlay != null) soldOverlay.SetActive(!hasChip);
            if (button != null) button.interactable = hasChip;

            if (!hasChip)
            {
                if (nameText != null) nameText.text = "SOLD";
                if (priceText != null) priceText.text = string.Empty;
                if (icon != null) icon.enabled = false;
                return;
            }

            if (icon != null) { icon.enabled = true; icon.sprite = chip.Icon; }
            if (nameText != null) nameText.text = chip.Name;
            if (priceText != null) priceText.text = chip.BasePrice.ToString();

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => _onBuy?.Invoke(_chip));
            }
        }
    }
}
