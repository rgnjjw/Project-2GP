using UnityEngine;

namespace _02_Scripts.Player
{
    public struct CameraShakeEvent
    {
        public Vector3 Velocity;
        public CameraShakeEvent(Vector3 velocity) => Velocity = velocity;
        public CameraShakeEvent(float strength, Vector3 direction) => Velocity = direction.normalized * strength;
    }
}
