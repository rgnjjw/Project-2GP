using UnityEngine;

namespace _02_Scripts.UI
{
    /// <summary>
    /// World Space 캔버스(전용 카메라 자식)에 있는 HUD 요소를, 카메라의 실제 시야(FOV/Aspect) 안의
    /// 뷰포트 좌표(0~1) 기준으로 배치한다. Safe Margin으로 가장자리에서 안쪽으로 띄워,
    /// 어떤 해상도/화면비에서도 좌우·상하가 절대 잘리지 않게 보장한다.
    ///
    /// 설계 의도:
    /// - World Space Canvas / 카메라 자식 구조 / 원근감(요소의 기울어진 회전·스케일) 그대로 유지.
    /// - 위치만 카메라 뷰포트에 맞춰 자동 재배치 → 화면비가 바뀌어도 항상 시야 안.
    /// - 각 HUD 요소(코너에 두는 부모 RectTransform)에 하나씩 붙인다.
    ///   예: InfoUI = (0,0) 좌하단, StyleUI = (1,1) 우상단.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class WorldSpaceHudAnchor : MonoBehaviour
    {
        [Tooltip("기준 카메라. 비우면 부모에서 Camera를 찾고, 없으면 Camera.main 사용.")]
        [SerializeField] private Camera targetCamera;

        [Tooltip("뷰포트 기준 위치. (0,0)=좌하단, (1,1)=우상단, (0.5,0.5)=중앙. 디자인 위치 그대로 넣으면 어떤 화면비에서도 그 비율로 유지된다.")]
        [SerializeField] private Vector2 viewportPosition = new Vector2(0f, 0f);

        [Tooltip("화면 가장자리에서 최소한 이만큼은 안쪽으로 보장(0~0.5). 범위를 벗어난 위치만 안쪽으로 당겨 절대 안 잘리게 한다.")]
        [Range(0f, 0.5f)]
        [SerializeField] private float safeMargin = 0.05f;

        [Tooltip("카메라 정면 기준 평면 거리(원근감·크기 결정). 0 이하면 시작 시 현재 거리를 자동 사용.")]
        [SerializeField] private float planeDistance = 0f;

        [Header("선택: 보통은 꺼둔다(디자인 보존)")]
        [Tooltip("요소를 카메라와 정면으로 정렬(기울어진 스타일 회전을 무시). 기본 꺼짐.")]
        [SerializeField] private bool alignToCamera = false;

        private RectTransform _rt;
        private float _planeDistance;
        private bool _distanceCaptured;

        private void Awake() => Init();
        private void OnEnable() { Init(); Apply(); }

        private void Init()
        {
            if (_rt == null) _rt = (RectTransform)transform;
        }

        private Camera ResolveCamera()
        {
            if (targetCamera != null) return targetCamera;
            var parentCam = GetComponentInParent<Camera>();
            if (parentCam != null) return parentCam;
            return Camera.main;
        }

        private float ResolvePlaneDistance(Camera cam)
        {
            if (planeDistance > 0.001f) return planeDistance;

            // 자동: 현재 요소가 카메라 정면 축으로 얼마나 떨어져 있는지 한 번 캡처해 고정.
            if (!_distanceCaptured)
            {
                float d = Vector3.Dot(_rt.position - cam.transform.position, cam.transform.forward);
                _planeDistance = d > 0.001f ? d : 10f;
                _distanceCaptured = true;
            }
            return _planeDistance;
        }

        private void LateUpdate() => Apply();

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rt = (RectTransform)transform;
            _distanceCaptured = false; // 인스펙터 수정 시 거리 재캡처
            // 즉시 한 번 적용해 에디터에서 바로 미리보기. (delayCall로 한 번 더 보정)
            Apply();
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this == null) return;
                    Apply();
                };
            }
        }
#endif

        private void Apply()
        {
            if (_rt == null) _rt = (RectTransform)transform;
            Camera cam = ResolveCamera();
            if (cam == null) return;

            // 지정한 뷰포트 위치를 그대로 쓰되, 안전 범위 [m, 1-m]를 벗어나면 그만큼만 안쪽으로 당긴다.
            // → 디자인 위치(화면비 안에 있던 값)는 그대로 유지되고, 가장자리를 넘어가는 것만 막아 절대 안 잘림.
            float m = Mathf.Clamp(safeMargin, 0f, 0.49f);
            float vx = Mathf.Clamp(viewportPosition.x, m, 1f - m);
            float vy = Mathf.Clamp(viewportPosition.y, m, 1f - m);

            float dist = ResolvePlaneDistance(cam);

            // 카메라 시야 안의 실제 월드 좌표. 해상도/화면비가 바뀌면 자동으로 달라진다 → 자동 재배치.
            Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(vx, vy, dist));
            _rt.position = worldPos;

            // 기본은 회전/스케일을 건드리지 않아 기울어진 스타일(원근감)을 보존한다.
            if (alignToCamera)
                _rt.rotation = cam.transform.rotation;
        }
    }
}
