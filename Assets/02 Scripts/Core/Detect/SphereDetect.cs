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
            var cols = Physics.OverlapSphere(trm.position, radius, layerMask);
            foreach (var col in cols)
            {
                if (col.transform != trm) return true;
            }
            return false;
        }
        public override Transform GetClosest(Transform trm)
        {
            Transform closest = null;
            float minDist = float.MaxValue;

            foreach (var col in Physics.OverlapSphere(trm.position, radius, layerMask))
            {
                if (col.transform == trm) continue;
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
            foreach (var col in Physics.OverlapSphere(trm.position, radius, layerMask))
            {
                if (col.transform != trm)
                    results.Add(col.transform);
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