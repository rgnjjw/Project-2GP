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
        private Color[] _originalEmissions;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

        private Tween _activeTween;

        public void Initialize(ModuleOwner owner)
        {
            _renderers = owner.GetComponentsInChildren<SkinnedMeshRenderer>();
            _mpb = new MaterialPropertyBlock();
            _originalEmissions = new Color[_renderers.Length];

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i].sharedMaterial.HasProperty(EmissionColor))
                {
                    _originalEmissions[i] = _renderers[i].sharedMaterial.GetColor(EmissionColor);
                }
                else
                {
                    _originalEmissions[i] = Color.black;
                }
            }
        }

        public void Flash()
        {
            _activeTween?.Kill();

            for (int i = 0; i < _renderers.Length; i++)
            {
                _renderers[i].GetPropertyBlock(_mpb);
                _mpb.SetColor(EmissionColor, flashColor);
                _renderers[i].SetPropertyBlock(_mpb);
            }

            _activeTween = DOVirtual.DelayedCall(flashDuration, () =>
            {
                for (int i = 0; i < _renderers.Length; i++)
                {
                    _renderers[i].GetPropertyBlock(_mpb);
                    _mpb.SetColor(EmissionColor, _originalEmissions[i]);
                    _renderers[i].SetPropertyBlock(_mpb);
                }
            });
        }
    }
}