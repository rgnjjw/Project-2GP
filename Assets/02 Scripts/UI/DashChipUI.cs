using System.Collections.Generic;
using _02_Scripts.Chip;
using _02_Scripts.Chip.Dash;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    public class DashChipUI : MonoBehaviour
    {
        [SerializeField] private ChipController chipController;
        [SerializeField] private GameObject iconPrefab;
        [SerializeField] private GameObject container;
        [SerializeField] private float fadeDuration = 0.25f;

        private readonly List<Image> _icons = new();
        private DashChip _dashChip;
        private int _currentLit;

        private void Awake()
        {
            chipController.OnChipEquipped += HandleChipEquipped;
            chipController.OnChipUnequipped += HandleChipUnequipped;
        }

        private void OnDestroy()
        {
            if (chipController == null) return;
            chipController.OnChipEquipped -= HandleChipEquipped;
            chipController.OnChipUnequipped -= HandleChipUnequipped;
            UnsubscribeDash();
        }

        private void HandleChipEquipped(IChip chip)
        {
            if (chip is not DashChip dashChip) return;
            _dashChip = dashChip;
            _dashChip.OnMaxCountChanged += HandleMaxCountChanged;
            _dashChip.OnDashUsed += HandleDashUsed;
            _dashChip.OnDashRecharged += HandleDashRecharged;
            container.SetActive(true);

            // OnEquip에서 이미 이벤트가 지나갔으므로 현재 상태로 직접 초기화
            HandleMaxCountChanged(_dashChip.CurrentDashCount, _dashChip.MaxDashCount);
        }

        private void HandleChipUnequipped(IChip chip)
        {
            if (chip is not DashChip) return;
            UnsubscribeDash();
            ClearIcons();
            container.SetActive(false);
        }

        private void HandleMaxCountChanged(int currentCount, int maxCount)
        {
            // 아이콘 부족하면 추가 생성
            while (_icons.Count < maxCount)
            {
                var go = Instantiate(iconPrefab, transform);
                var img = go.GetComponent<Image>();
                img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
                _icons.Add(img);
            }

            // 현재 활성화 수에 맞게 알파 즉시 세팅 (장착 직후 or 레벨업 직후)
            for (int i = 0; i < _icons.Count; i++)
            {
                float targetAlpha = i < currentCount ? 1f : 0f;
                var c = _icons[i].color;
                _icons[i].color = new Color(c.r, c.g, c.b, targetAlpha);
            }

            _currentLit = currentCount;
        }

        private void HandleDashUsed()
        {
            if (_currentLit <= 0) return;
            _currentLit--;
            _icons[_currentLit].DOFade(0f, fadeDuration);
        }

        private void HandleDashRecharged()
        {
            if (_currentLit >= _icons.Count) return;
            _icons[_currentLit].DOFade(1f, fadeDuration);
            _currentLit++;
        }

        private void UnsubscribeDash()
        {
            if (_dashChip == null) return;
            _dashChip.OnMaxCountChanged -= HandleMaxCountChanged;
            _dashChip.OnDashUsed -= HandleDashUsed;
            _dashChip.OnDashRecharged -= HandleDashRecharged;
            _dashChip = null;
        }

        private void ClearIcons()
        {
            foreach (var img in _icons)
                if (img != null) Destroy(img.gameObject);
            _icons.Clear();
            _currentLit = 0;
        }
    }
}
