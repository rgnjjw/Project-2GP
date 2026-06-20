using System.Collections;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    public class EnemyBulletBeamSKillSO : MonoBehaviour
    {
        private LineRenderer _lr;
        private Coroutine _hideRoutine;

        public void Setup(Material material, float width)
        {
            if (_lr != null) return;
            _lr = gameObject.AddComponent<LineRenderer>();
            _lr.positionCount = 2;
            _lr.useWorldSpace = true;
            _lr.widthMultiplier = width;
            if (material != null) _lr.material = material;
            _lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _lr.receiveShadows = false;
            _lr.enabled = false;
        }

        public void Show(Vector3 start, Vector3 end, float duration)
        {
            _lr.SetPosition(0, start);
            _lr.SetPosition(1, end);
            _lr.enabled = true;
            if (_hideRoutine != null) StopCoroutine(_hideRoutine);
            _hideRoutine = StartCoroutine(HideAfter(duration));
        }

        private IEnumerator HideAfter(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (_lr != null) _lr.enabled = false;
        }
    }
}
