using _02_Scripts.Agent;
using _02_Scripts.Core.Detect;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Enemy.Skill;

namespace _02_Scripts.Enemy
{
    public class EnemyDataContainer : AgentDataContainer<EnemyDataSO>,IAfterInitModule
    {
        public SkillSO[] EnemySkills { get; private set; }
        public AbstractDetection ChaseRange { get; private set; }

        // 처치 보상(적별). Enemy.OnDead에서 사용.
        public int ExpReward => initDataSO.ExpReward;
        public float StyleReward => initDataSO.StyleReward;
        public int CurrencyReward => initDataSO.CurrencyReward;

        private NavEnemyRenderer _navEnemyRenderer;

        public override void Initialize(ModuleOwner owner)
        {
            base.Initialize(owner);
            EnemySkills = initDataSO.EnemySkills;
            ChaseRange = initDataSO.ChaseRange;

            if (owner is Enemy enemy)
            {
                _navEnemyRenderer = enemy.GetModule<NavEnemyRenderer>();
            }
        }

        public void AfterInit()
        {
            if(_navEnemyRenderer == null) return;
            _navEnemyRenderer.NavMeshAgent.speed = MoveSpeed.Value;
        }
    }
}