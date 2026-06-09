using _02_Scripts.Enemy.Skill;
using _02_Scripts.Enemy.State;
using _02_Scripts.Manager;
using _02_Scripts.UI;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class Enemy : Agent.Agent
    {
        public Transform CurrentTarget { get; set; }
        private EnemySkillController _enemySkillController;
        private NavEnemyRenderer _navEnemyRenderer;
        protected override void Awake()//풀링후에 Enable로 수정
        {
            base.Awake();
            ChangeState(EnemyStateEnum.IDLE);
            _enemySkillController = GetModule<EnemySkillController>();
            _navEnemyRenderer = GetModule<NavEnemyRenderer>();
            _navEnemyRenderer.NavMeshAgent.stoppingDistance = _enemySkillController.GetMinSkillRange() - 0.2f;
        }

        // protected override void OnEnable()
        // {
        //     base.OnEnable();
        //     ChangeState(EnemyStateEnum.IDLE);
        // }


        protected override void OnDead()
        {
            StageManager.Instance.EnemyCount--;
            LevelManager.Instance.AddExp(10);
            Destroy(gameObject);
        }
        public void ChangeState(EnemyStateEnum nextState) => stateMachine.ChangeState((int)nextState);
        
    }
}