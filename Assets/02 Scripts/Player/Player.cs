using DG.Tweening;
using _02_Scripts.Agent;
using _02_Scripts.Manager;
using _02_Scripts.Player.FSM;
using _02_Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using EventBus = _02_Scripts.Core.Utility.EventBus;

namespace _02_Scripts.Player
{
    public class Player : Agent.Agent
    {
        [field: SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
        public bool IsWallRiding { get; set; }
        public event System.Action OnWallRideEnded;
        public void FireWallRideEnded() => OnWallRideEnded?.Invoke();
        [SerializeField] private BarUI barUI;
        [SerializeField] private Image redPanel;
        [SerializeField] private RecoilEvent recoilEvent;

        private Controls _controls;
        private GunManager _gunManager;
        private AgentHealth _playerHealth;

        private Tween _damageTween;

        protected override void Awake()
        {
            base.Awake();

            _controls = new Controls();
            _controls.Player.SetCallbacks(PlayerInputSO);
            _controls.Player.Enable();

            _gunManager = GetModule<GunManager>();
            _playerHealth = GetModule<AgentHealth>();

            PlayerInputSO.OnFireKeyPressed += _gunManager.Fire;
            _playerHealth.CurrentHp.OnValueChanged += OnHpChanged;

            Color color = redPanel.color;
            color.a = 0f;
            redPanel.color = color;

            ChangeState(PlayerStateEnum.IDLE);
        }

        private void OnHpChanged(int before, int current)
        {
            barUI.SetFill((float)current / _playerHealth.MaxHp);

            if (current < before)
                PlayDamageEffect();
            
            EventBus.Publish(recoilEvent);
        }

        private void PlayDamageEffect()
        {
            _damageTween?.Kill();

            Color color = redPanel.color;
            color.a = 0.4f;
            redPanel.color = color;

            _damageTween = redPanel
                .DOFade(0f, 0.3f)
                .SetEase(Ease.OutQuad);
        }

        protected override void OnDead()
        {

        }

        protected void OnDestroy()
        {
            PlayerInputSO.OnFireKeyPressed -= _gunManager.Fire;
            _playerHealth.CurrentHp.OnValueChanged -= OnHpChanged;
        }

        public void ChangeState(PlayerStateEnum nextState)
            => stateMachine.ChangeState((int)nextState);
    }
}