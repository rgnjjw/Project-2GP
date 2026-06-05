using _02_Scripts.Gun;
using _02_Scripts.Player.FSM;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class Player : Agent.Agent
    {
        [field: SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
        private Controls _controls;
        private GunManager _gunManager;
        protected override void Awake()
        {
            base.Awake();
            _controls = new Controls();
            _controls.Player.SetCallbacks(PlayerInputSO);
            _controls.Player.Enable();

            //총 바인딩
            _gunManager = GetModule<GunManager>();
            PlayerInputSO.OnFireKeyPressed += _gunManager.Fire;
            
            ChangeState(PlayerStateEnum.IDLE);
        }

        protected override void OnDead()
        {
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerInputSO.OnFireKeyPressed -= _gunManager.Fire;
        }

        public void ChangeState(PlayerStateEnum nextState)=> stateMachine.ChangeState((int)nextState);
    }
}