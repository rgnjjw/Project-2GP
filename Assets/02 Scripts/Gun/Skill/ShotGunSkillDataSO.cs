using System;
using UnityEngine;

namespace _02_Scripts.Gun.Skill
{
    [CreateAssetMenu(fileName = "ShotGunSkillDataSO", menuName = "Gun/Skill/ShotGunSkillDataSO")]
    public class ShotGunSkillDataSO : ScriptableObject
    {
        [Serializable]
        public class LevelData
        {
            public float DamagePerSecond = 40f;
            public float Range = 3f;
            public float Cooldown = 6f;
        }

        [field: SerializeField] public LevelData[] Levels { get; private set; }
        [field: SerializeField] public LayerMask EnemyMask { get; private set; }

        public LevelData GetLevel(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, Levels.Length - 1);
            return Levels[idx];
        }
    }
}
