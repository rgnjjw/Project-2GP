using _02_Scripts.Agent;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Player;

namespace _02_Scripts.Map
{
    public class PlayerDataContainer : AgentDataContainer<PlayerDataSO>
    {
        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
        }
    }
}