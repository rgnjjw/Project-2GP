using _02_Scripts.Gun;
using _02_Scripts.Manager;
using csiimnida.CSILib.SoundManager.RunTime;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Map
{
    // 상점의 다운도어 버튼. 총으로 쏘면 두 문이 열리고 랜덤 맵으로 다음 스테이지를 시작한다.
    // 이 오브젝트(또는 자식)에 Collider가 있어야 하고, 그 레이어가 총 layerMask에 포함돼야 한다.
    [RequireComponent(typeof(Collider))]
    public class StageDoorButton : MonoBehaviour, IShootable
    {
        [Header("두 문 (열릴 때 이동할 Transform)")]
        [SerializeField] private Transform leftDoor;
        [SerializeField] private Transform rightDoor;
        [Tooltip("닫힌 위치 기준 왼쪽 문이 열릴 때 이동하는 로컬 오프셋")]
        [SerializeField] private Vector3 leftOpenOffset = new Vector3(-2f, 0f, 0f);
        [Tooltip("닫힌 위치 기준 오른쪽 문이 열릴 때 이동하는 로컬 오프셋")]
        [SerializeField] private Vector3 rightOpenOffset = new Vector3(2f, 0f, 0f);
        [SerializeField] private float openDuration = 0.6f;
        [SerializeField] private Ease openEase = Ease.OutCubic;

        [Header("동작 조건")]
        [Tooltip("스테이지를 클리어한 상태에서만 작동(상점에서만 열림).")]
        [SerializeField] private bool requireStageCleared = true;
        [Tooltip("쏘면 재생할 사운드 키(SoundManager). 비우면 사운드 없음.")]
        [SerializeField] private string openSoundKey = "";

        private Vector3 _leftClosed;
        private Vector3 _rightClosed;

        private void Awake()
        {
            if (leftDoor != null) _leftClosed = leftDoor.localPosition;
            if (rightDoor != null) _rightClosed = rightDoor.localPosition;
        }

        public void OnShot()
        {
            // 클리어 상태가 아니면(전투 중) 무시. StartNextStage가 즉시 IsStageCleared=false로 바꾸므로
            // 샷건 펠릿처럼 한 프레임에 여러 발 맞아도 한 번만 시작된다.
            if (requireStageCleared && (StageManager.Instance == null || !StageManager.Instance.IsStageCleared))
                return;

            OpenDoors();

            if (!string.IsNullOrEmpty(openSoundKey) && SoundManager.Instance != null)
                SoundManager.Instance.PlaySound(openSoundKey);

            StageManager.Instance.StartNextStage();
        }

        private void OpenDoors()
        {
            if (leftDoor != null)
            {
                leftDoor.DOKill();
                leftDoor.DOLocalMove(_leftClosed + leftOpenOffset, openDuration).SetEase(openEase);
            }
            if (rightDoor != null)
            {
                rightDoor.DOKill();
                rightDoor.DOLocalMove(_rightClosed + rightOpenOffset, openDuration).SetEase(openEase);
            }
        }
    }
}
