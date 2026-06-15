using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        public EnemyType Type;

        private void OnDrawGizmos()
        {
            Gizmos.color = Type switch
            {
                EnemyType.Melee => Color.red,
                EnemyType.Ranged => Color.blue,
                EnemyType.Boss => Color.yellow,
                _ => Color.white
            };
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
