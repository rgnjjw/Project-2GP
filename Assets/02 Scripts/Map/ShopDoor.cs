using _02_Scripts.Manager;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class ShopDoor : MonoBehaviour
    {
        [SerializeField] private Door[] doors;
        [SerializeField] private Vector3 detectSize = new Vector3(3f, 2f, 3f);
        [SerializeField] private Vector3 detectOffset;
        [SerializeField] private LayerMask playerLayer;
        
        
        private bool _isOpen;

        private void FixedUpdate()
        {
            bool playerInRange = Physics.OverlapBox(
                transform.position + detectOffset,
                detectSize * 0.5f,
                transform.rotation,
                playerLayer).Length > 0;

            bool shouldOpen = playerInRange && IsStageCleared();

            if (shouldOpen && !_isOpen)
            {
                _isOpen = true;
                foreach (var door in doors)
                    door.Open();
            }
            else if (!shouldOpen && _isOpen)
            {
                _isOpen = false;
                foreach (var door in doors)
                    door.Close();
            }
        }

        private static bool IsStageCleared()
            => StageManager.Instance != null && StageManager.Instance.IsStageCleared;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.matrix = Matrix4x4.TRS(transform.position + detectOffset, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, detectSize);
        }
    }
}