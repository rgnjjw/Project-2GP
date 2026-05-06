using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.Chip.Card
{
    public class ChipCardButtonUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button button;

        private ChipInstance _chip;
        private Action<ChipInstance> _onSelected;

        public void Setup(ChipInstance chip, Action<ChipInstance> onSelected)
        {
            _chip = chip;
            _onSelected = onSelected;

            int previewIndex = chip.IsEquipped
                ? Mathf.Min(chip.CurrentLevel, chip.Data.MaxLevel - 1)
                : 0;
            LevelData levelData = chip.Data.LevelNameAndDescData[previewIndex];

            if (icon != null) icon.sprite = chip.Data.Icon;
            nameText.text = levelData.Name;
            descriptionText.text = levelData.Description;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnClick);
        }

        public void SetInteractable(bool value) => button.interactable = value; //버튼 활성 비활성화

        private void OnClick() => _onSelected?.Invoke(_chip);
    }
}
