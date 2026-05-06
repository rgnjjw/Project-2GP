using System;
using _02_Scripts.Manager;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapSelector : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Player"))
                CursorManager.Instance.SetCursorVisible(true);
        }

        private void OnDisable()
        {
            CursorManager.Instance.SetCursorVisible(false);
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.gameObject.CompareTag("Player"))
                CursorManager.Instance.SetCursorVisible(false);
        }
    }
}