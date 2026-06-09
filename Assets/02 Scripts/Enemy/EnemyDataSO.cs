using _02_Scripts.Agent;
using _02_Scripts.Core.Detect;
using _02_Scripts.Enemy.Skill;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    [CreateAssetMenu(fileName = "EnemyDataSO", menuName = "Enemy/DataSO", order = 0)]
    public class EnemyDataSO : AgentDataSO
    {
        [field: SerializeReference] public AbstractDetection ChaseRange { get; set; }
        [field: SerializeField] public SkillSO[] EnemySkills { get; private set; }
    }
}