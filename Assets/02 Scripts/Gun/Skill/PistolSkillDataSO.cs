using System;
using UnityEngine;

namespace _02_Scripts.Gun.Skill
{
    [CreateAssetMenu(fileName = "PistolSkillDataSO", menuName = "Gun/Skill/PistolSkillDataSO")]
    public class PistolSkillDataSO : ScriptableObject
    {
        [Serializable]
        public class LevelData
        {
            public int Damage = 30;
            public float BulletSpeed = 20f;
            public float AutoDeleteTime = 5f;
            public float Cooldown = 8f;
        }

        [field: SerializeField] public LevelData[] Levels { get; private set; }
        [field: SerializeField] public LayerMask HitMask { get; private set; }

        public LevelData GetLevel(int level)
        {
            int idx = Mathf.Clamp(level - 1, 0, Levels.Length - 1);
            return Levels[idx];
        }
    }
}
