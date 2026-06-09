using System;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Core.Detect
{
    [Serializable]
    public abstract class AbstractDetection
    {
        [SerializeField] protected LayerMask layerMask;
        [SerializeField] protected Color gizmoColor;
        public abstract float Range { get; }
        
        protected AbstractDetection() { }

        public abstract bool HasAnyInRange(Transform trm);
        public abstract Transform GetClosest(Transform trm);
        public abstract List<Transform> GetAllInRange(Transform trm);
        public abstract void DrawGizmos(Transform trm);
    }
}