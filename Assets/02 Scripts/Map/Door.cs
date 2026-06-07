using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class Door : MonoBehaviour
    {
        [HideInInspector] public float Duration;
        [SerializeField] private Vector3 endPos;
        private Vector3 _startPos;

        private void Awake()
        {
            _startPos = transform.position;
        }

        public void Open()
        {
            transform.DOMove(endPos, Duration);
        }

        public void Close()
        {
            transform.DOMove(_startPos, Duration);
        }
    }
}