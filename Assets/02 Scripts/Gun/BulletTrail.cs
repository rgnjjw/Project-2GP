using UnityEngine;

namespace _02_Scripts.Gun
{
    public class BulletTrail : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float speed = 200f;

        private Vector3 _target;
        private bool _moving;

        public void Initialize(Vector3 start, Vector3 target)
        {
            transform.position = start;
            _target = target;
            _moving = true;
        }

        private void Update()
        {
            if (!_moving) return;

            transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _target) < 0.01f)
            {
                _moving = false;
                Destroy(gameObject, trailRenderer.time);
            }
        }
    }
}