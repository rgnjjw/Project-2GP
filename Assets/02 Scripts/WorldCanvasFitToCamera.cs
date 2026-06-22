using UnityEngine;

namespace _02_Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class WorldCanvasFitToCamera : MonoBehaviour
    {
        private Camera parentCamera;
        private RectTransform rectTransform;
        private float lastAspect;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            parentCamera = GetComponentInParent<Camera>();
        }

        void Start()
        {
            FitCanvas();
        }

        void Update()
        {
            if (!Mathf.Approximately(parentCamera.aspect, lastAspect))
            {
                FitCanvas();
            }
        }

        void FitCanvas()
        {
            float distance = Mathf.Abs(rectTransform.localPosition.z);
            float frustumHeight = 2f * distance * Mathf.Tan(parentCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * parentCamera.aspect;

            rectTransform.sizeDelta = new Vector2(frustumWidth, frustumHeight);
            lastAspect = parentCamera.aspect;
        }
    }
}