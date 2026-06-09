using System.Collections.Generic;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    public class EnemySkillController : MonoBehaviour, IModule, IAfterInitModule
    {
        public IReadOnlyList<SkillSO> Skills => _skills;
        private List<SkillSO> _skills;
        private Dictionary<SkillSO, float> _cooldownDict;
        private Enemy _enemy;

        public void Initialize(ModuleOwner owner)
        {
            if (owner is Enemy enemy)
                _enemy = enemy;
        }

        public void AfterInit()
        {
            var data = _enemy.GetModule<EnemyDataContainer>();
            _skills = new List<SkillSO>(data.EnemySkills);
            _cooldownDict = new Dictionary<SkillSO, float>(_skills.Count);

            foreach (var skill in _skills)
                _cooldownDict[skill] = float.MinValue;
        }

        public SkillSO GetAvailableSkill()
        {
            SkillSO closestSkill = null;
            float minRange = float.MaxValue;

            foreach (var skill in _skills)
            {
                if (!IsOffCooldown(skill) || !skill.CanExecuteSkill(_enemy.transform)) continue;
                if (skill.TargetFinder == null) continue;

                if (skill.TargetFinder.Range < minRange)
                {
                    minRange = skill.TargetFinder.Range;
                    closestSkill = skill;
                }
            }
            return closestSkill;
        }
        
        public float GetMinSkillRange()
        {
            float minRange = float.MaxValue;
            foreach (var skill in _skills)
            {
                if (skill.TargetFinder == null) continue;
                if (skill.TargetFinder.Range < minRange)
                    minRange = skill.TargetFinder.Range;
            }
            return Mathf.Approximately(minRange, float.MaxValue) ? 0f : minRange;
        }
        private bool IsOffCooldown(SkillSO skill)
            => Time.time >= _cooldownDict[skill] + skill.Cooldown;

        public void RecordSkillUsed(SkillSO skill)
            => _cooldownDict[skill] = Time.time;
    }
}