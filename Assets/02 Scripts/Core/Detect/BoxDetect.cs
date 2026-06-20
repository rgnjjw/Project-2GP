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
            int count = Physics.OverlapBoxNonAlloc(GetCenter(trm), size * 0.5f, OverlapBuffer, trm.rotation, layerMask);
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

            int count = Physics.OverlapBoxNonAlloc(GetCenter(trm), size * 0.5f, OverlapBuffer, trm.rotation, layerMask);
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
            int count = Physics.OverlapBoxNonAlloc(GetCenter(trm), size * 0.5f, OverlapBuffer, trm.rotation, layerMask);
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
            Matrix4x4 prev = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(GetCenter(trm), trm.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.matrix = prev;
        }
    }
}