using _02_Scripts.Agent;

namespace _02_Scripts.Enemy.State
{
    public class AbstractEnemyState : AgentState
    {
        protected readonly Enemy enemy;
        protected AbstractEnemyState(Agent.Agent agent, int clipHash) : base(agent, clipHash)
        {
            if (agent is Enemy enemyCompo)
                enemy = enemyCompo;
        }
    }
}