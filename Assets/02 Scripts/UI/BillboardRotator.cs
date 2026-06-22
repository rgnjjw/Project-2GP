using UnityEngine;

namespace _02_Scripts.UI
{
    public class BillboardRotator : MonoBehaviour
    {
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            // 씬 재시작 등으로 카메라가 늦게 생기거나 교체되면 다시 잡는다.
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null) return;
            }

            // 카메라 회전을 그대로 따라가 항상 화면과 평행하게 만든다(즉시·스냅).
            // (이전엔 LookRotation(pos - camPos)라 위/아래에서 볼 때 막대가 상하로 기울어져 이상하게 보였다.)
            transform.rotation = _camera.transform.rotation;
        }
    }
}
