using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace _02_Scripts.Enemy.Skill
{
    [Serializable]
    public struct SummonEntry
    {
        public GameObject Prefab;
        public int Weight;
    }

    [CreateAssetMenu(fileName = "SummonSkillSO", menuName = "Skill/SummonSkillSO", order = 0)]
    public class SummonSkillSO : SkillSO
    {
        [field: SerializeField] public int SummonCount { get; private set; } = 3;
        [field: SerializeField] public float SpawnRadius { get; private set; } = 5f;
        [field: SerializeField] public float MinSpacing { get; private set; } = 1.5f;
        [field: SerializeField] public int MaxAttempts { get; private set; } = 20;
        [SerializeField] private SummonEntry[] summonPool;

        public override void ExecuteSkill(Enemy enemy)
        {
            enemy.GetModule<EnemyVfxController>()?.Play(EnemyVfxType.Summon);

            var spawnedPositions = new List<Vector3>();

            for (int i = 0; i < SummonCount; i++)
            {
                GameObject prefab = GetWeightedRandom();
                if (prefab == null) continue;

                if (!TryGetSpawnPosition(enemy.transform.position, spawnedPositions, out Vector3 spawnPos)) continue;

                spawnedPositions.Add(spawnPos);

                Vector3 startPos = new Vector3(spawnPos.x, spawnPos.y - 3f, spawnPos.z);
                GameObject obj = UnityEngine.Object.Instantiate(prefab, startPos, Quaternion.identity);

                var navAgent = obj.GetComponent<NavMeshAgent>();
                if (navAgent != null) navAgent.enabled = false;

                obj.transform.DOMoveY(spawnPos.y, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    if (navAgent != null)
                    {
                        navAgent.enabled = true;
                        navAgent.Warp(spawnPos);
                    }
                });
            }

            NotifyComplete();
        }

        private bool TryGetSpawnPosition(Vector3 center, List<Vector3> occupied, out Vector3 result)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                Vector2 rand = UnityEngine.Random.insideUnitCircle * SpawnRadius;
                Vector3 candidate = center + new Vector3(rand.x, 0f, rand.y);

                if (!NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    continue;

                bool tooClose = false;
                foreach (var pos in occupied)
                {
                    if (Vector3.Distance(hit.position, pos) < MinSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose) continue;

                result = hit.position;
                return true;
            }

            result = Vector3.zero;
            return false;
        }

        private GameObject GetWeightedRandom()
        {
            if (summonPool == null || summonPool.Length == 0) return null;

            int totalWeight = 0;
            foreach (var entry in summonPool)
                totalWeight += entry.Weight;

            int rand = UnityEngine.Random.Range(0, totalWeight);
            int cumulative = 0;

            foreach (var entry in summonPool)
            {
                cumulative += entry.Weight;
                if (rand < cumulative)
                    return entry.Prefab;
            }

            return summonPool[0].Prefab;
        }
    }
}