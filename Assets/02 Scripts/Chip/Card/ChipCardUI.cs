using System;
using System.Collections.Generic;
using _02_Scripts.Manager;
using _02_Scripts.UI;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Chip.Card
{
    [Serializable]
    public class StageCardChipData
    {
        public ChipDataSO ChipData;
    }

    public class ChipCardUI : MonoBehaviour
    {
        [SerializeField] private ChipCardButtonUI[] cards;
        [SerializeField] private StageCardChipData[] stageChipData;
        [SerializeField] private GameObject panel;
        [SerializeField] private ChipController chipController;

        [SerializeField] private float enterDuration = 0.45f;
        [SerializeField] private float enterStagger  = 0.08f;
        [SerializeField] private float exitDuration  = 0.35f;
        
        // 카드 선택 창이 떠 있는 동안 true. (일시정지 등 다른 입력이 끼어들지 못하게 막는 용도)
        public static bool IsSelecting { get; private set; }

        private RectTransform _rectTransform;
        private Vector2 _originalPosition;
        private ChipInstance[] _stageChipInstances;
        private float[] _cardOriginalY;

        private void Awake()
        {
            LevelManager.Instance.OnLevelUp += ShowCards;
        }

        private void OnDestroy()
        {
            LevelManager.Instance.OnLevelUp -= ShowCards;
        }

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalPosition = _rectTransform.anchoredPosition;
            
            _stageChipInstances = new ChipInstance[stageChipData.Length];
            for (int i = 0; i < stageChipData.Length; i++)
                _stageChipInstances[i] = new ChipInstance(stageChipData[i].ChipData);

            _cardOriginalY = new float[cards.Length];
            for (int i = 0; i < cards.Length; i++)
                _cardOriginalY[i] = cards[i].GetComponent<RectTransform>().anchoredPosition.y;

            panel.SetActive(false);

            if (chipController != null) ShowCards();
        }

        public void ShowCards(int level = 0)
        {
            var available = GetAvailableChips();
            if (available.Count == 0) return;

            Shuffle(available);
            int count = Mathf.Min(cards.Length, available.Count);

            IsSelecting = true;
            Time.timeScale = 0f;
            panel.SetActive(true);
            CursorManager.Instance.SetCursorVisible(true);
            Canvas.ForceUpdateCanvases();

            float canvasHeight = ((RectTransform)panel.transform).rect.height;

            for (int i = 0; i < cards.Length; i++)
            {
                bool active = i < count;
                cards[i].gameObject.SetActive(active);
                cards[i].SetInteractable(false);

                if (!active) continue;

                int captured = i;
                cards[captured].Setup(available[captured], chip => OnCardSelected(chip, captured));

                RectTransform rt = cards[captured].GetComponent<RectTransform>();
                rt.DOKill(true);
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, -canvasHeight);
            }

            PlayEnterAnimation(count);
        }

        private void PlayEnterAnimation(int count)
        {
            Sequence enterSeq = DOTween.Sequence().SetUpdate(true);

            for (int i = 0; i < count; i++)
            {
                int captured = i;
                RectTransform rt = cards[captured].GetComponent<RectTransform>();
                enterSeq.Insert(
                    captured * enterStagger,
                    rt.DOAnchorPosY(_cardOriginalY[captured], enterDuration).SetEase(Ease.OutBack).SetUpdate(true)
                );
            }

            enterSeq.OnComplete(() =>
            {
                for (int i = 0; i < count; i++)
                    cards[i].SetInteractable(true);
            });
        }

        private List<ChipInstance> GetAvailableChips()
        {
            var list = new List<ChipInstance>();
            foreach (var chip in _stageChipInstances)
            {
                if (!chip.IsEquipped || chip.CurrentLevel < chip.Data.MaxLevel)
                    list.Add(chip);
            }
            return list;
        }

        private void OnCardSelected(ChipInstance chip, int cardIndex)
        {
            if (!chip.IsEquipped)
                chipController.EquipChip(chip);
            else
                chipController.ChipLevelUp(chip);

            for (int i = 0; i < cards.Length; i++)
                cards[i].SetInteractable(false);

            CursorManager.Instance.SetCursorVisible(true);

            cards[cardIndex].GetComponent<RectTransform>()
                .DOPunchScale(Vector3.one * 0.25f, 0.3f, 0, 0.5f).SetUpdate(true)
                .OnComplete(PlayExitAnimation);
        }

        private void PlayExitAnimation()
        {
            float canvasHeight = ((RectTransform)panel.transform).rect.height;
            Sequence exitSeq = DOTween.Sequence().SetUpdate(true);

            for (int i = 0; i < cards.Length; i++)
            {
                if (!cards[i].gameObject.activeSelf) continue;
                RectTransform rt = cards[i].GetComponent<RectTransform>();
                rt.DOKill(true);
                exitSeq.Join(rt.DOAnchorPosY(-canvasHeight, exitDuration).SetEase(Ease.InBack).SetUpdate(true));
            }

            exitSeq.OnComplete(() =>
            {
                Time.timeScale = 1f;
                panel.SetActive(false);
                _rectTransform.anchoredPosition = _originalPosition;
                CursorManager.Instance.SetCursorVisible(false);
                IsSelecting = false;
            });
        }

        private static void Shuffle<T>(List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}