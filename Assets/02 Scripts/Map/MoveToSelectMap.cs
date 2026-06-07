using UnityEngine;

namespace _02_Scripts.Map
{
    public class MoveToSelectMap : MonoBehaviour
    { 
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private Transform point;
        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & targetLayer.value) != 0)
            {
                other.transform.position = point.position;
            }
        }
    }
}