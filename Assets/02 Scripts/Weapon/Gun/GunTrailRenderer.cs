using System.Collections;
using UnityEngine;

namespace _02_Scripts.Weapon.Gun
{
    public class GunTrailRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer;

        public void DrawTrail(Vector3[] points)
        {
            StartCoroutine(DrawCoroutine(points));
        }

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