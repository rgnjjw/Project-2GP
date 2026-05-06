using _02_Scripts.Manager;

namespace _02_Scripts.Enemy
{
    public class Enemy : Agent.Agent
    {
        protected override void OnDead()
        {
            StageManager.Instance.EnemyCount--;
            StageManager.Instance.ChipCardUI.ShowCards();
            Destroy(gameObject);
        }
    }
}