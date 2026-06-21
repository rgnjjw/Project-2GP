using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Vector3 openPosition;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private Ease ease = Ease.OutQuad;

        private Vector3 _closedPosition;

        private void Awake()
        {
            _closedPosition = transform.localPosition;
        }

        public void Open()
        {
            transform.DOLocalMove(openPosition, duration).SetEase(ease);
        }

        public void Close()
        {
            transform.DOLocalMove(_closedPosition, duration).SetEase(ease);
        }
    }
}
