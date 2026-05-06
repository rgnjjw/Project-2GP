using DG.Tweening;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Player
{
    public struct CameraShakeEvent
    {
        public float Duration;
        public float Strength;
        public int Vibrato;
    }

    public class CameraShaker : MonoBehaviour
    {
        private void OnEnable()
        {
            EventBus.Subscribe<CameraShakeEvent>(OnShake);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CameraShakeEvent>(OnShake);
        }

        private void OnShake(CameraShakeEvent evt)
        {
            transform.DOKill(true);
            transform.DOShakePosition(evt.Duration, evt.Strength, evt.Vibrato, 90f, false, true);
        }
    }
}
