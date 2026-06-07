    using System;
    using System.Collections;
    using _02_Scripts.Core.ModuleSystem;
    using UnityEngine;

    namespace _02_Scripts.Map
    {
        public class DoorTrigger : MonoBehaviour
        {
            [SerializeField] private LayerMask targetLayer;
            [SerializeField] private Door[] doors;
            [SerializeField] private float duration;
            [SerializeField] private float delayTime;

            private void Awake()
            {
                foreach (var door in doors)
                {
                    door.Duration = duration;
                }
            }

            private void OnTriggerEnter(Collider other)
            {
                if (((1 << other.gameObject.layer) & targetLayer.value) != 0)
                {
                    StartCoroutine(OpenRoutine());
                }
            }

            private void OnTriggerExit(Collider other)
            {
                if (((1 << other.gameObject.layer) & targetLayer.value) != 0)   
                {
                    StartCoroutine(CloseRoutine());
                }
            }

            public void Open()
            {
                StartCoroutine(OpenRoutine());
            }

            public void Close()
            {
                StartCoroutine(CloseRoutine());
            }

            private IEnumerator OpenRoutine()
            {
                doors[0].Open();
                doors[1].Open();
                yield return new WaitForSeconds(delayTime);
                doors[2].Open();
                doors[3].Open();
            }

            private IEnumerator CloseRoutine()
            {
                doors[3].Close();
                doors[2].Close();
                yield return new WaitForSeconds(delayTime);
                doors[1].Close();
                doors[0].Close();
            }
        }
    }