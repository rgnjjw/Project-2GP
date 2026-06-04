using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Player.FSM.States
{
    public abstract class AbstractPlayerState : AgentState
    {
        protected Player player;
        protected PlayerVisualController visualController;
        protected AbstractPlayerState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            if(agent is Player player)
            {
                this.player = player;
                visualController = player.GetModule<PlayerVisualController>();
            }
        }

        public override void Enter(float transitionDuration, int layerIndex = 0)
        {
            base.Enter(transitionDuration, layerIndex);
            visualController.CurrentVisual.PlayClip(_stateClipHash,0, transitionDuration, 0);
        }
    }
}