using _02_Scripts.Player.FSM;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class Player : Agent.Agent
    {
        [field: SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
        protected override void OnDead()
        {
            throw new System.NotImplementedException();
        }
        public void ChangeState(PlayerStateEnum nextState)=> stateMachine.ChangeState((int)nextState);
    }
}