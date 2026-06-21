using System;
using _02_Scripts.Agent;
using UnityEngine;

namespace _02_Scripts
{
    public class DeadZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<Player.Player>(out var player))
                {
                    player.GetModule<AgentHealth>().ApplyDamage(100000);
                }
            }
        }
    }
}