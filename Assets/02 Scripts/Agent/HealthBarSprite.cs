using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public class HealthBarSprite : MonoBehaviour
    {
       [SerializeField] private AgentHealth agentHealth;
       [SerializeField] private Transform scaleHandler;
       
       [SerializeField] private bool useSmooth;
       [SerializeField] private float smoothSpeed;
       private float _targetFill;
       [SerializeField] private bool destroyOnDeath;
       [SerializeField] private float healthBarDisableTime = 2f;
       [SerializeField] private float fadeOutDuration = 0.5f;
       
       [Header("Color")] 
       [SerializeField] private SpriteRenderer fillRenderer;
       [SerializeField] private Color fullColor = Color.green;
       [SerializeField] private Color emptyColor = Color.red;
       private Color _targetColor;
       private Coroutine _activeCoroutine;

       private SpriteRenderer[] _allRenderers;

       private void Awake()
       {
          if (agentHealth == null)
             agentHealth = GetComponentInParent<AgentHealth>();

          _allRenderers = GetComponentsInChildren<SpriteRenderer>(true);
          
          agentHealth.CurrentHp.OnValueChanged += HandleHealthChange;
          agentHealth.OnDead += HandleDeath;

          ApplyFill(GetFill(), true);
       }
       
       private void OnDestroy()
       {
          if(_activeCoroutine != null) StopCoroutine(_activeCoroutine);
          
          agentHealth.CurrentHp.OnValueChanged -= HandleHealthChange;
          agentHealth.OnDead -= HandleDeath;
       }

       private void LateUpdate()
       {
          if (!useSmooth || scaleHandler == null) return;
          
          Vector3 scale = scaleHandler.localScale;
          if (Mathf.Approximately(scale.x, _targetFill)) return;
 
          scale.x = Mathf.Lerp(scale.x, _targetFill, Time.deltaTime * smoothSpeed);
          scaleHandler.localScale = scale;
          
          if (fillRenderer != null && fillRenderer.color != _targetColor)
             fillRenderer.color = Color.Lerp(fillRenderer.color, _targetColor, Time.deltaTime * smoothSpeed);
       }

       private void ApplyFill(float fill, bool immediate = false)
       {
          _targetFill = Mathf.Clamp01(fill);
          _targetColor = Color.Lerp(emptyColor, fullColor, _targetFill);
 
          if (immediate || !useSmooth)
          {
             Vector3 scale = scaleHandler.localScale;
             scale.x = _targetFill;
             scaleHandler.localScale = scale;
          }
       }

       private void HandleDeath()
       {
          if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);

          foreach (var sr in _allRenderers)
             sr.DOFade(0f, fadeOutDuration);

          if (destroyOnDeath)
             Destroy(gameObject, fadeOutDuration);
          else
             DOVirtual.DelayedCall(fadeOutDuration, () => gameObject.SetActive(false));
       }

       private void HandleHealthChange(int before, int current)
       {
          ApplyFill(current / Mathf.Max(agentHealth.MaxHp, 1f));
          if (_activeCoroutine != null) StopCoroutine(_activeCoroutine);
          gameObject.SetActive(true);
          _activeCoroutine = StartCoroutine(ActiveCoroutine(false));
       }

       private IEnumerator ActiveCoroutine(bool value)
       {
          yield return new WaitForSeconds(healthBarDisableTime);
          gameObject.SetActive(value);
       }

       private float GetFill()
          => agentHealth.CurrentHp.Value / (float)Mathf.Max(agentHealth.MaxHp, 1);

       public void ResetForPool()
       {
          DOTween.Kill(gameObject);

          foreach (var sr in _allRenderers)
          {
             Color c = sr.color;
             c.a = 1f;
             sr.color = c;
          }

          ApplyFill(GetFill(), true);
          gameObject.SetActive(false);
       }
    }
}