using UnityEngine;
using _02_Scripts.Chip;

namespace _02_Scripts.UI
{
    public class StaminaBarUI : MonoBehaviour
    {
        [SerializeField] private BarUI barUI;
        [SerializeField] private ChipController chipController;
        [SerializeField] private GameObject container;

        private IStaminaProvider _staminaProvider;

        private void Awake()
        {
            chipController.OnChipEquipped += HandleChipEquipped;
            chipController.OnChipUnequipped += HandleChipUnequipped;
        }

        private void Update()
        {
            if (_staminaProvider == null) return;
            barUI.SetFillRealtime(_staminaProvider.Stamina / _staminaProvider.MaxStamina);
        }

        private void HandleChipEquipped(IChip chip)
        {
            if (chip is not IStaminaProvider provider) return;
            _staminaProvider = provider;
            container.SetActive(true);
        }

        private void HandleChipUnequipped(IChip chip)
        {
            if (chip is not IStaminaProvider) return;
            _staminaProvider = null;
            container.SetActive(false);
        }

        private void OnDestroy()
        {
            if (chipController == null) return;
            chipController.OnChipEquipped -= HandleChipEquipped;
            chipController.OnChipUnequipped -= HandleChipUnequipped;
        }
    }
}
