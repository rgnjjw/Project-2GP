using UnityEngine;

namespace _02_Scripts.UI
{
    // 월드 스페이스 UI(캔버스)가 화면 해상도/화면비와 무관하게 항상 카메라 안에 다 들어오도록
    // 자동으로 스케일을 맞춰준다. (좁은 화면에선 줄여서 안 잘리게, 기본 디자인 크기보다 커지진 않음)
    // 월드 캔버스 루트(RectTransform)에 붙이면 된다.
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class WorldCanvasFitter : MonoBehaviour
    {
        [Tooltip("기준 카메라. 비우면 Camera.main 사용.")]
        [SerializeField] private Camera targetCamera;

        [Tooltip("화면에서 UI가 차지할 최대 비율(여백). 0.9 = 화면의 90%까지만 채움.")]
        [Range(0.1f, 1f)]
        [SerializeField] private float screenFillRatio = 0.9f;

        [Tooltip("가로 폭을 화면에 맞출지")]
        [SerializeField] private bool fitWidth = true;
        [Tooltip("세로 높이를 화면에 맞출지")]
        [SerializeField] private bool fitHeight = true;

        private RectTransform _rt;
        private Vector3 _baseScale;

        private void Awake()
        {
            _rt = (RectTransform)transform;
            _baseScale = _rt.localScale;
            if (_baseScale == Vector3.zero) _baseScale = Vector3.one;
        }

        private void OnEnable() => Fit();
        private void LateUpdate() => Fit();

        private void Fit()
        {
            Camera cam = targetCamera != null ? targetCamera : Camera.main;
            if (cam == null) return;

            // 캔버스가 카메라 앞으로 얼마나 떨어져 있는지(카메라 정면 축 기준 거리)
            float distance = Vector3.Dot(_rt.position - cam.transform.position, cam.transform.forward);
            if (distance <= 0.001f) return;

            // 그 거리에서 카메라가 실제로 보여주는 월드 크기(세로/가로)
            float viewH, viewW;
            if (cam.orthographic)
            {
                viewH = cam.orthographicSize * 2f;
                viewW = viewH * cam.aspect;
            }
            else
            {
                viewH = 2f * distance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
                viewW = viewH * cam.aspect;
            }

            // 캔버스의 디자인 월드 크기(기준 스케일 기준)
            float uiW = _rt.rect.width * Mathf.Abs(_baseScale.x);
            float uiH = _rt.rect.height * Mathf.Abs(_baseScale.y);
            if (uiW <= 0f || uiH <= 0f) return;

            // 가로/세로 모두 화면 안에 들어오는 가장 작은 배율을 택한다(둘 중 더 빡빡한 쪽 기준 → 절대 안 잘림).
            float scale = 1f; // 기본 디자인 크기보다 커지지 않게 1로 시작
            if (fitWidth) scale = Mathf.Min(scale, (viewW * screenFillRatio) / uiW);
            if (fitHeight) scale = Mathf.Min(scale, (viewH * screenFillRatio) / uiH);

            _rt.localScale = _baseScale * scale;
        }
    }
}
