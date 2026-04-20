using UnityEngine;

namespace _02_Scripts.Player.FSM.States
{
    public class PlayerMove : AbstractPlayerState
    {
        private PlayerMover _playerMover;
        public PlayerMove(Agent.Agent agent, int clipHash) : base(agent, clipHash)
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
            
            Vector3 moveDir = forward * zInput + right * xInput;

            moveDir = moveDir.normalized;

            _playerMover.SetMovementX(moveDir.x);
            _playerMover.SetMovementZ(moveDir.z);

            if (Mathf.Abs(xInput) < 0.1f && Mathf.Abs(zInput) < 0.1f)
            {
                player.ChangeState(PlayerStateEnum.IDLE);
            }
        }

    }
}