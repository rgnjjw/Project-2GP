using _02_Scripts.Agent;
using _02_Scripts.Gun;
using _02_Scripts.Manager;
using _02_Scripts.Player.FSM;
using _02_Scripts.UI;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class Player : Agent.Agent
    {
        [field: SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
        [SerializeField] private BarUI barUI;
        private Controls _controls;
        private GunManager _gunManager;
        private AgentHealth _playerHealth;
        
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
            
            ChangeState(PlayerStateEnum.IDLE);
        }

        private void OnHpChanged(int before, int current)
        {
            barUI.SetFill((float)current / _playerHealth.MaxHp);
        }

        protected override void OnDead()
        {
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerInputSO.OnFireKeyPressed -= _gunManager.Fire;
            _playerHealth.CurrentHp.OnValueChanged -= OnHpChanged;
        }

        public void ChangeState(PlayerStateEnum nextState) => stateMachine.ChangeState((int)nextState);
    }
}