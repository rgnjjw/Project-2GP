using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Player.FSM.States
{
    public abstract class AbstractPlayerState : AgentState
    {
        protected Player player;
        protected AbstractPlayerState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            player = agent as Player;
        }
    }
}