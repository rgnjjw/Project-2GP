using _02_Scripts.Player.FSM;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class Player : Agent.Agent
    {
        [field: SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
        private Controls _controls;
        protected override void Awake()
        {
            base.Awake();
            _controls = new Controls();
            _controls.Player.SetCallbacks(PlayerInputSO);
            _controls.Player.Enable(); 
            
            ChangeState(PlayerStateEnum.IDLE);
        }
        protected override void OnDead() { }
        public void ChangeState(PlayerStateEnum nextState)=> stateMachine.ChangeState((int)nextState);
    }
}