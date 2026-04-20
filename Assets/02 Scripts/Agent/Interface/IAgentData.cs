using _02_Scripts.Core.Utility;

namespace _02_Scripts.Agent.Interface
{
    public interface IAgentData
    {
        public NotifyValue<int> Health { get; }
        public NotifyValue<int> MaxHealth { get; }
        public NotifyValue<float> MoveSpeed{ get; }
    }
}