// ConeDetect.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Core.Detect
{
    [Serializable]
    public class ConeDetect : AbstractDetection
    {
        [SerializeField] private float angle;
        [SerializeField] private float radius;

        public override float Range => radius;

        public ConeDetect() { }

        public override bool HasAnyInRange(Transform trm)
        {
            float halfAngle = angle * 0.5f;
            foreach (var col in Physics.OverlapSphere(trm.position, radius, layerMask))
            {
                if (col.transform == trm) continue;
                Vector3 dir = (col.transform.position - trm.position).normalized;
                if (Vector3.Angle(trm.forward, dir) <= halfAngle)
                    return true;
            }
            return false;
        }

        public override Transform GetClosest(Transform trm)
        {
            Transform closest = null;
            float minDist = float.MaxValue;
            float halfAngle = angle * 0.5f;

            foreach (var col in Physics.OverlapSphere(trm.position, radius, layerMask))
            {
                if (col.transform == trm) continue;
                Vector3 dir = (col.transform.position - trm.position).normalized;
                if (Vector3.Angle(trm.forward, dir) > halfAngle) continue;

                float dist = Vector3.Distance(trm.position, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = col.transform;
                }
            }
            return closest;
        }

        public override List<Transform> GetAllInRange(Transform trm)
        {
            var results = new List<Transform>();
            float halfAngle = angle * 0.5f;

            foreach (var col in Physics.OverlapSphere(trm.position, radius, layerMask))
            {
                if (col.transform == trm) continue;
                Vector3 dir = (col.transform.position - trm.position).normalized;
                if (Vector3.Angle(trm.forward, dir) <= halfAngle)
                    results.Add(col.transform);
            }
            return results;
        }

        public override void DrawGizmos(Transform trm)
        {
            Gizmos.color = gizmoColor;
            Vector3 forward = trm.forward;
            float halfAngle = angle * 0.5f;
            int segments = 20;
            Vector3 prevPoint = trm.position + Quaternion.Euler(0, -halfAngle, 0) * forward * radius;

            for (int i = 1; i <= segments; i++)
            {
                float t = (float)i / segments;
                float currentAngle = Mathf.Lerp(-halfAngle, halfAngle, t);
                Vector3 nextPoint = trm.position + Quaternion.Euler(0, currentAngle, 0) * forward * radius;
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }

            Gizmos.DrawLine(trm.position, trm.position + Quaternion.Euler(0, -halfAngle, 0) * forward * radius);
            Gizmos.DrawLine(trm.position, trm.position + Quaternion.Euler(0,  halfAngle, 0) * forward * radius);
        }
    }
}