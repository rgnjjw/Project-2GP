using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Core.Detect
{
    [Serializable]
    public class BoxDetect : AbstractDetection
    {
        [SerializeField] private Vector3 size = Vector3.one;
        [SerializeField] private Vector3 offset = Vector3.zero;

        public override float Range => Mathf.Max(size.x, size.y, size.z) * 0.5f;

        public BoxDetect() { }

        private Vector3 GetCenter(Transform trm)
            => trm.position + trm.TransformDirection(offset);

        public override bool HasAnyInRange(Transform trm)
        {
            var cols = Physics.OverlapBox(GetCenter(trm), size * 0.5f, trm.rotation, layerMask);
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

            foreach (var col in Physics.OverlapBox(GetCenter(trm), size * 0.5f, trm.rotation, layerMask))
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
            foreach (var col in Physics.OverlapBox(GetCenter(trm), size * 0.5f, trm.rotation, layerMask))
            {
                if (col.transform != trm)
                    results.Add(col.transform);
            }
            return results;
        }

        public override void DrawGizmos(Transform trm)
        {
            Gizmos.color = gizmoColor;
            Matrix4x4 prev = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(GetCenter(trm), trm.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = prev;
        }
    }
}