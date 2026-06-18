using System;
using UnityEngine;

namespace _02_Scripts.Gun.Skill
{
    [CreateAssetMenu(fileName = "MachineGunSkillDataSO", menuName = "Gun/Skill/MachineGunSkillDataSO")]
    public class MachineGunSkillDataSO : ScriptableObject
    {
        [Serializable]
        public class LevelData
        {
            public float Duration = 4f;
            public float Cooldown = 10f;
        }

        [field: SerializeField] public LevelData[] Levels { get; private set; }

        public LevelData GetLevel(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, Levels.Length - 1);
            return Levels[idx];
        }
    }
}
