using System;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill
{
    public class SkillGizmoRender : MonoBehaviour
    {
        [SerializeField] private Transform userTrm;
        [SerializeField] private EnemyDataSO enemyDataSO;

        private void OnDrawGizmos()
        {
            if(userTrm == null || enemyDataSO == null) return;
            
            foreach (var skill in enemyDataSO.EnemySkills)
            {
                skill.DamageAreaDetection?.DrawGizmos(userTrm);
                skill.TargetFinder?.DrawGizmos(userTrm);
            }
            
            enemyDataSO.ChaseRange?.DrawGizmos(userTrm);
        }
    }
}