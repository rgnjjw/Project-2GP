using System.Collections;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class GunTrailRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;
        private Transform _muzzleTrm;

        public void DrawTrail(Vector3[] points)
        {
            Vector3[] finalPoints = new Vector3[points.Length];
            finalPoints[0] = _muzzleTrm.position;
            for (int i = 1; i < points.Length; i++)
                finalPoints[i] = points[i];

            StartCoroutine(DrawCoroutine(finalPoints));
        }

        public void SetMuzzleTrm(Transform muzzleTrm) => _muzzleTrm = muzzleTrm;
        

        private IEnumerator DrawCoroutine(Vector3[] points)
        {
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
            lineRenderer.enabled = true;

            yield return new WaitForSeconds(0.01f);

            lineRenderer.enabled = false;
        }
    }
}