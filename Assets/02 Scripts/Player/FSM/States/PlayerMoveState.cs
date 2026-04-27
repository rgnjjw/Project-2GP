using UnityEngine;

namespace _02_Scripts.Player.FSM.States
{
    public class PlayerMoveState : AbstractPlayerState
    {
        private readonly PlayerMover _playerMover;
        public PlayerMoveState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            _playerMover = agent.GetModule<PlayerMover>();
        }

        public override void Update()
        {
            base.Update();
            float xInput = player.PlayerInputSO.InputDirection.x;
            float zInput = player.PlayerInputSO.InputDirection.y;

            Vector3 forward = player.transform.forward; 
            Vector3 right = player.transform.right;
            
            if (xInput == 0f && zInput == 0f)
            {
                _playerMover.SetMovementX(0f);
                _playerMover.SetMovementZ(0f);
                player.ChangeState(PlayerStateEnum.IDLE);
                return;
            }
            
            Vector3 moveDir = forward * zInput + right * xInput;

            _playerMover.SetMovementX(moveDir.x);
            _playerMover.SetMovementZ(moveDir.z);
        }

    }
}