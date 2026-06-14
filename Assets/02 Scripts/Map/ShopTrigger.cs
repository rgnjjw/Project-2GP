using System;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class ShopTrigger : MonoBehaviour
    {
        [SerializeField] private Transform targetPos;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.transform.position = targetPos.position;
            }
        }
    }
}