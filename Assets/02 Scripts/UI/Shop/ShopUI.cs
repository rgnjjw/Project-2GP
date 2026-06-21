using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Chip;
using _02_Scripts.Chip.Weapon;
using _02_Scripts.Manager;
using _02_Scripts.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI.Shop
{
    // 상점 화면 총괄.
    // - 상단: 랜덤 패시브칩 3개 구매 (처음 한 번만 추첨, 리롤 없음, 구매하면 그 칸은 비워짐)
    // - 하단: 보유 칩 전체 표시 (무기 / 행동 / 패시브 3분할)
    //   · 패시브칩 클릭 → "강화하시겠습니까?" 확인 → 예 누르면 강화 (풀렙이면 "풀렙")
    //   · 무기칩 / 행동칩 클릭 → "강화 불가능" (인게임에서만 강화)
    public class ShopUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private ShopChipCatalogSO catalog;
        [SerializeField] private ChipController chipController;

        [Header("구매(랜덤 패시브 3)")]
        [SerializeField] private ShopPurchaseCardUI[] purchaseCards;

        [Header("보유 칩 목록")]
        [SerializeField] private ShopOwnedChipEntryUI entryPrefab;
        [SerializeField] private Transform weaponContainer;
        [SerializeField] private Transform actionContainer;
        [SerializeField] private Transform passiveContainer;

        [Header("강화 확인 다이얼로그")]
        [SerializeField] private GameObject confirmPanel;
        [SerializeField] private TMP_Text confirmText;
        [SerializeField] private Button confirmYesButton;
        [SerializeField] private Button confirmNoButton;

        [Header("재화 표시")]
        [SerializeField] private TMP_Text currencyText;

        [Header("안내 메시지")]
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private float messageDuration = 1.5f;

        private ShopChipDataSO[] _drawnPassives;   // 3칸, 구매되면 null
        private bool _drawn;
        private ShopChipDataSO _pendingUpgrade;
        private Coroutine _messageRoutine;

        private void Awake()
        {
            if (confirmYesButton != null) confirmYesButton.onClick.AddListener(OnConfirmYes);
            if (confirmNoButton != null) confirmNoButton.onClick.AddListener(OnConfirmNo);

            if (confirmPanel != null) confirmPanel.SetActive(false);
            if (messageText != null) messageText.text = string.Empty;
        }

        private void OnEnable()
        {
            if (PlayerChipInventory.Instance != null)
                PlayerChipInventory.Instance.OnInventoryChanged += RefreshOwned;
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnCurrencyChanged += OnCurrencyChanged;

            // 상점이 켜질 때(ShopCanvas 활성화) 자동으로 채운다 — 외부 Open() 호출이 없어도 동작.
            Refresh();
        }

        private void OnDisable()
        {
            if (PlayerChipInventory.Instance != null)
                PlayerChipInventory.Instance.OnInventoryChanged -= RefreshOwned;
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.OnCurrencyChanged -= OnCurrencyChanged;
        }

        private void OnCurrencyChanged(int amount) => RefreshCurrency();

        private void RefreshCurrency()
        {
            if (currencyText != null && CurrencyManager.Instance != null)
                currencyText.text = CurrencyManager.Instance.Currency.ToString();
        }

        // 외부(상점 진입 버튼/트리거)에서 호출.
        public void Open()
        {
            if (shopPanel != null) shopPanel.SetActive(true);
            Refresh();
        }

        // 현재 상태로 화면을 다시 그린다.
        private void Refresh()
        {
            RefreshCurrency();
            EnsureDrawn();
            RefreshPurchase();
            RefreshOwned();
        }

        public void Close()
        {
            if (confirmPanel != null) confirmPanel.SetActive(false);
            if (shopPanel != null) shopPanel.SetActive(false);
        }

        // ---------- 구매 ----------

        private void EnsureDrawn()
        {
            if (_drawn) return;
            _drawn = true;
            _drawnPassives = new ShopChipDataSO[purchaseCards != null ? purchaseCards.Length : 3];

            if (catalog == null) return;

            // 아직 보유하지 않은 패시브칩만 후보로 모은다.
            var candidates = new List<ShopChipDataSO>();
            var inventory = PlayerChipInventory.Instance;
            foreach (var chip in catalog.PassiveChips)
            {
                if (chip == null) continue;
                if (inventory != null && inventory.IsOwned(chip.ChipId)) continue;
                candidates.Add(chip);
            }

            Shuffle(candidates);
            int count = Mathf.Min(_drawnPassives.Length, candidates.Count);
            for (int i = 0; i < count; i++)
                _drawnPassives[i] = candidates[i];
        }

        private void RefreshPurchase()
        {
            if (purchaseCards == null) return;
            for (int i = 0; i < purchaseCards.Length; i++)
            {
                if (purchaseCards[i] == null) continue;
                ShopChipDataSO chip = _drawnPassives != null && i < _drawnPassives.Length ? _drawnPassives[i] : null;
                purchaseCards[i].Setup(chip, OnBuyClicked);
            }
        }

        private void OnBuyClicked(ShopChipDataSO chip)
        {
            if (chip == null || ShopManager.Instance == null) return;

            if (ShopManager.Instance.TryPurchase(chip))
            {
                // 구매한 칸은 비운다(리롤 없음).
                for (int i = 0; i < _drawnPassives.Length; i++)
                    if (_drawnPassives[i] == chip) _drawnPassives[i] = null;

                RefreshPurchase();
                RefreshOwned();
            }
            else
            {
                ShowMessage("구매 불가 (재화 부족/보유 중)");
            }
        }

        // ---------- 보유 칩 표시 ----------

        private void RefreshOwned()
        {
            ClearContainer(weaponContainer);
            ClearContainer(actionContainer);
            ClearContainer(passiveContainer);

            // 무기칩 + 행동칩: ChipController의 장착 리스트에서 가져와 타입으로 분류.
            if (chipController != null)
            {
                foreach (ChipInstance chip in chipController.GetEquippedChips())
                {
                    if (chip?.Data == null) continue;

                    bool isWeapon = chip.Data is WeaponChipDataSO;
                    Transform parent = isWeapon ? weaponContainer : actionContainer;
                    string level = $"Lv.{chip.CurrentLevel}/{chip.Data.MaxLevel}";

                    SpawnEntry(parent, chip.Data.Icon, chip.Data.Name, level, ShowCannotUpgrade);
                }
            }

            // 패시브칩: 인벤토리 보유 목록(id→레벨)에서 카탈로그로 SO 역참조.
            var inventory = PlayerChipInventory.Instance;
            if (inventory != null && catalog != null)
            {
                foreach (KeyValuePair<string, int> kv in inventory.GetOwnedPassives())
                {
                    ShopChipDataSO so = catalog.GetById(kv.Key);
                    if (so == null) continue;

                    string level = $"Lv.{kv.Value}/{so.MaxLevel}";
                    ShopChipDataSO captured = so;
                    SpawnEntry(passiveContainer, so.Icon, so.Name, level, () => OnPassiveClicked(captured));
                }
            }
        }

        private void SpawnEntry(Transform parent, Sprite icon, string name, string level, System.Action onClick)
        {
            if (entryPrefab == null || parent == null) return;
            ShopOwnedChipEntryUI entry = Instantiate(entryPrefab, parent);
            entry.Setup(icon, name, level, onClick);
        }

        private static void ClearContainer(Transform container)
        {
            if (container == null) return;
            for (int i = container.childCount - 1; i >= 0; i--)
                Destroy(container.GetChild(i).gameObject);
        }

        // ---------- 강화 ----------

        private void OnPassiveClicked(ShopChipDataSO chip)
        {
            var inventory = PlayerChipInventory.Instance;
            if (inventory == null) return;

            int current = inventory.GetCurrentLevel(chip.ChipId);
            if (current >= chip.MaxLevel)
            {
                ShowMessage("풀렙");
                return;
            }

            _pendingUpgrade = chip;
            if (confirmText != null)
            {
                int price = chip.GetUpgradePrice(current);
                confirmText.text = $"{chip.Name} 강화하시겠습니까?\n(비용 {price})";
            }
            if (confirmPanel != null) confirmPanel.SetActive(true);
        }

        private void OnConfirmYes()
        {
            if (_pendingUpgrade != null && ShopManager.Instance != null)
            {
                if (!ShopManager.Instance.TryUpgrade(_pendingUpgrade))
                    ShowMessage("강화 실패 (재화 부족)");
            }
            _pendingUpgrade = null;
            if (confirmPanel != null) confirmPanel.SetActive(false);
            RefreshOwned();
        }

        private void OnConfirmNo()
        {
            _pendingUpgrade = null;
            if (confirmPanel != null) confirmPanel.SetActive(false);
        }

        // 무기칩/행동칩은 상점에서 강화 불가.
        private void ShowCannotUpgrade() => ShowMessage("강화 불가능");

        // ---------- 유틸 ----------

        private void ShowMessage(string text)
        {
            if (messageText == null) return;
            messageText.text = text;
            if (_messageRoutine != null) StopCoroutine(_messageRoutine);
            _messageRoutine = StartCoroutine(ClearMessageAfter());
        }

        private IEnumerator ClearMessageAfter()
        {
            yield return new WaitForSecondsRealtime(messageDuration);
            if (messageText != null) messageText.text = string.Empty;
            _messageRoutine = null;
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
