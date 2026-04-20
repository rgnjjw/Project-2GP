using _02_Scripts.Agent.Interface;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public class AgentDataContainer<T> : MonoBehaviour,IModule,IAgentData where T : AgentDataSO
    {
        [field: SerializeField] public T InitDataSO { get; private set; }
        [field: SerializeField] public NotifyValue<int> Health { get; private set; }
        [field: SerializeField] public NotifyValue<int> MaxHealth { get; private set; }
        [field: SerializeField] public NotifyValue<float> MoveSpeed { get; private set; }
        public virtual void Initialize(ModuleOwner owner)
        {
            Health = new NotifyValue<int>(InitDataSO.Health);
            MaxHealth = new NotifyValue<int>(InitDataSO.Health);
            MoveSpeed = new NotifyValue<float>(InitDataSO.MoveSpeed);
        }

    }
}