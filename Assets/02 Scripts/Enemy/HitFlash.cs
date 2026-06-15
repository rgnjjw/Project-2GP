using _02_Scripts.Core.ModuleSystem;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class HitFlash : MonoBehaviour, IModule
    {
        [SerializeField] private float flashDuration = 0.1f;
        [SerializeField] private Color flashColor = Color.white;

        private SkinnedMeshRenderer[] _renderers;
        private MaterialPropertyBlock _mpb;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        public void Initialize(ModuleOwner owner)
        {
            _renderers = owner.GetComponentsInChildren<SkinnedMeshRenderer>();
            _mpb = new MaterialPropertyBlock();
        }

        public void Flash()
        {
            foreach (var r in _renderers)
            {
                r.GetPropertyBlock(_mpb);
                _mpb.SetColor(EmissionColor, flashColor);
                r.SetPropertyBlock(_mpb);
            }

            DOVirtual.DelayedCall(flashDuration, () =>
            {
                foreach (var r in _renderers)
                {
                    r.GetPropertyBlock(_mpb);
                    _mpb.SetColor(EmissionColor, Color.black);
                    r.SetPropertyBlock(_mpb);
                }
            });
        }
    }
}