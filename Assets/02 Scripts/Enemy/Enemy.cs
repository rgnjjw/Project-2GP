using _02_Scripts.Manager;
using _02_Scripts.UI;

namespace _02_Scripts.Enemy
{
    public class Enemy : Agent.Agent
    {
        protected override void OnDead()
        {
            StageManager.Instance.EnemyCount--;
            LevelManager.Instance.AddExp(10);
            Destroy(gameObject);
        }
    }
}