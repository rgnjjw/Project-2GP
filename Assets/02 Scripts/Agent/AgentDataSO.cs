using _02_Scripts.Agent.Interface;
using UnityEngine;

namespace _02_Scripts.Agent
{
    public class AgentDataSO : ScriptableObject
    {
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public int MaxHealth { get; private set; }
        [field: SerializeField] public float MoveSpeed { get; private set; }
    }
}