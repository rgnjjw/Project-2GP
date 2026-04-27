using System;
using UnityEngine;

namespace _02_Scripts.Agent.Interface
{
    public interface IMover
    {
        bool IsGrounded { get; }
        event Action<bool> OnGroundStatusChanged;
        event Action<Vector3> OnVelocityChanged;
        void SetMoveSpeedMultiplier(float value);
        void AddForceToAgent(Vector3 force);
        void StopImmediately(bool xAxis, bool yAxis, bool zAxis);
        void SetMovementX(float value);
        void SetMovementZ(float value);
    }
}