using UnityEngine;

namespace _02_Scripts.Player.FSM.States
{
    public class PlayerIdleState : AbstractPlayerState
    {
        private readonly PlayerMover _playerMover;
        public PlayerIdleState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            _playerMover = player.GetModule<PlayerMover>();
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            _playerMover.StopImmediately(true,false,true);
        }

        public override void Update()
        { 
            base.Update();
            float xInput = player.PlayerInputSO.InputDirection.x;
            float zInput = player.PlayerInputSO.InputDirection.y;
         
            if (xInput != 0 || zInput != 0)
            {
                player.ChangeState(PlayerStateEnum.MOVE);
            }
        }
        
        
    }
}