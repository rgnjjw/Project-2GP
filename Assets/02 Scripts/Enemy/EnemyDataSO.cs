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

        [Header("처치 보상 (적마다 다르게)")]
        [Tooltip("처치 시 주는 경험치.")]
        [field: SerializeField] public int ExpReward { get; private set; } = 10;
        [Tooltip("처치 시 오르는 스타일 점수(0~100 게이지 기준).")]
        [field: SerializeField] public float StyleReward { get; private set; } = 10f;
        [Tooltip("처치 시 주는 재화. 0이면 안 줌(돈은 주로 스테이지 등급 보상으로).")]
        [field: SerializeField] public int CurrencyReward { get; private set; } = 0;
    }
}