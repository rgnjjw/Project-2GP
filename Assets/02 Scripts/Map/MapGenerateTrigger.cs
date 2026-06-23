using _02_Scripts.Manager;
using UnityEngine;

namespace _02_Scripts.Map
{
    [RequireComponent(typeof(Collider))]
    public class MapGenerateTrigger : MonoBehaviour
    {
        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (StageManager.Instance == null) return;

            StageManager.Instance.StartNextStage();
        }
    }
}
