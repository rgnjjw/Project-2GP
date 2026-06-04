using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts.Player
{
    public class PlayerRenderer : AgentRenderer
    {
        [field:SerializeField] public PlayerVisualState visualState;
    }
}