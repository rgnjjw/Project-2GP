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
            if (_camera == null) return;

            Vector3 dir = transform.position - _camera.transform.position;
            if (dir.sqrMagnitude > 0f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }
}