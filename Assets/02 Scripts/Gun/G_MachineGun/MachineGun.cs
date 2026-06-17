using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Gun.G_MachineGun
{
    public class MachineGun : Gun
    {
        [SerializeField] private Transform muzzleRight;
        [SerializeField] private Transform muzzleLeft;
        [SerializeField] private ParticleSystem fireEffectRight;
        [SerializeField] private ParticleSystem fireEffectLeft;

        public override bool IsAutoFire => true;
        public bool LastFiredLeft { get; private set; }

        private bool _isLeft;

        public override void Fire()
        {
            if (Camera.main == null) return;
            if (Time.time < nextFireTime) return;

            nextFireTime = Time.time + fireDelay;

            LastFiredLeft = _isLeft;
            Transform currentMuzzle = _isLeft ? muzzleLeft : muzzleRight;
            _isLeft = !_isLeft;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

            var tracer = Instantiate(tracerEffect, currentMuzzle.position, Quaternion.identity);
            tracer.AddPosition(currentMuzzle.position);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                hitEffect.transform.position = hit.point;
                hitEffect.transform.forward = hit.normal;
                hitEffect.Emit(1);

                tracer.transform.position = hit.point;

                if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                    DealDamage(enemy.GetModule<AgentHealth>(), bulletDamage);
            }
            else
            {
                tracer.transform.position = ray.origin + ray.direction * 1000f;
            }

            base.Fire();
        }

        protected override void PlayFireEffect()
        {
            (LastFiredLeft ? fireEffectLeft : fireEffectRight)?.Play();
        }
    }
}
