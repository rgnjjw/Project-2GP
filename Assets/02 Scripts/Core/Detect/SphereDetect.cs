// SphereDetect.cs
using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Core.Detect
{
    [Serializable]
    public class SphereDetect : AbstractDetection
    {
        [SerializeField] private float radius;

        public override float Range => radius;

        public SphereDetect() { }

        public override bool HasAnyInRange(Transform trm)
        {
            int count = Physics.OverlapSphereNonAlloc(trm.position, radius, OverlapBuffer, layerMask);
            for (int i = 0; i < count; i++)
            {
                if (OverlapBuffer[i].transform != trm) return true;
            }
            return false;
        }
        public override Transform GetClosest(Transform trm)
        {
            Transform closest = null;
            float minDist = float.MaxValue;

            int count = Physics.OverlapSphereNonAlloc(trm.position, radius, OverlapBuffer, layerMask);
            for (int i = 0; i < count; i++)
            {
                Transform t = OverlapBuffer[i].transform;
                if (t == trm) continue;
                float dist = (trm.position - t.position).sqrMagnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = t;
                }
            }
            return closest;
        }

        public override List<Transform> GetAllInRange(Transform trm)
        {
            var results = new List<Transform>();
            int count = Physics.OverlapSphereNonAlloc(trm.position, radius, OverlapBuffer, layerMask);
            for (int i = 0; i < count; i++)
            {
                if (OverlapBuffer[i].transform != trm)
                    results.Add(OverlapBuffer[i].transform);
            }
            return results;
        }

        public override void DrawGizmos(Transform trm)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(trm.position, radius);
        }
    }
}