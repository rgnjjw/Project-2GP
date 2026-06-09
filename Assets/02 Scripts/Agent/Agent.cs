using _02_Scripts.Core.FSMSystem;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public abstract class Agent : ModuleOwner
    {
        [SerializeField] protected StateListSO stateList;
        
        protected StateMachine stateMachine;
        protected override void InitializeModules()
        {
            base.InitializeModules();
            stateMachine = new StateMachine(this,stateList.states);
        }

        protected virtual void Update()
        {
            stateMachine.UpdateMachine();
        }

        protected virtual void OnEnable()
        {
            GetModule<AgentHealth>().OnDead += OnDead;
        }

        protected virtual void OnDisable()
        {
            GetModule<AgentHealth>().OnDead -= OnDead;
        }

        protected abstract void OnDead();
        public AgentState GetCurrentState() => stateMachine.CurrentState;
    }
}