using _02_Scripts.Agent.Interface;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Core.Utility;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public abstract class AgentDataContainer<T> : MonoBehaviour,IModule,IAgentData where T : AgentDataSO
    {
        [SerializeField] private T initDataSO;
        public NotifyValue<int> Health { get; private set;}
        public NotifyValue<int> MaxHealth { get; private set;}
        public NotifyValue<float> MoveSpeed { get; private set;}
        public virtual void Initialize(ModuleOwner owner)
        {
            Health = new NotifyValue<int>(initDataSO.Health);
            MaxHealth = new NotifyValue<int>(initDataSO.Health);
            MoveSpeed = new NotifyValue<float>(initDataSO.MoveSpeed);
        }
    }
}