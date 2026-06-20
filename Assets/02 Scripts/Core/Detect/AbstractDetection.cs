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

        // 모든 Detection이 공유하는 물리 질의 버퍼.
        // OverlapXXXNonAlloc 결과를 담아 매 호출마다 배열을 새로 할당(GC)하지 않도록 한다.
        // 메인 스레드에서 동기적으로만 사용되고, 결과를 즉시 소비하므로 공유해도 안전하다.
        protected static readonly Collider[] OverlapBuffer = new Collider[64];
        
        protected AbstractDetection() { }

        public abstract bool HasAnyInRange(Transform trm);
        public abstract Transform GetClosest(Transform trm);
        public abstract List<Transform> GetAllInRange(Transform trm);
        public abstract void DrawGizmos(Transform trm);
    }
}