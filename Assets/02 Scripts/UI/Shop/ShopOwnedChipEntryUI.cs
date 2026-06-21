using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI.Shop
{
    // 상점 "보유 칩" 목록의 항목 한 칸. (무기/행동/패시브 공용)
    public class ShopOwnedChipEntryUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private Button button;

        public void Setup(Sprite iconSprite, string chipName, string levelLabel, Action onClick)
        {
            if (icon != null) { icon.enabled = iconSprite != null; icon.sprite = iconSprite; }
            if (nameText != null) nameText.text = chipName;
            if (levelText != null) levelText.text = levelLabel;

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onClick?.Invoke());
            }
        }
    }
}
