using System;
using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Agent;
using _02_Scripts.Enemy;
using _02_Scripts.Map;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02_Scripts.Manager
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private EnemyTableSO enemyTable;

        public event Action OnAllWavesCleared;

        private MapDataSO _mapData;
        private int _waveIndex;
        private int _livingEnemyCount;

        public void SetMapData(MapDataSO mapData) => _mapData = mapData;

        public void StartWaves(int currentLevel)
        {
            _waveIndex = 0;
            _livingEnemyCount = 0;
            StartCoroutine(RunWaves(currentLevel));
        }

        private IEnumerator RunWaves(int currentLevel)
        {
            while (_waveIndex < _mapData.Waves.Length)
            {
                yield return StartCoroutine(SpawnWave(_mapData.Waves[_waveIndex], currentLevel));
                yield return new WaitUntil(() => _livingEnemyCount <= 0);
                _waveIndex++;
            }
            OnAllWavesCleared?.Invoke();
        }

        private IEnumerator SpawnWave(WaveDataSO wave, int currentLevel)
        {
            EnemyLevelData levelData = enemyTable.GetData(currentLevel);
            if (levelData == null) yield break;

            SpawnPointData[] allPoints = _mapData.SpawnPoints;
            List<SpawnPointData> meleePoints = FilterPoints(allPoints, EnemyType.Melee);
            List<SpawnPointData> rangedPoints = FilterPoints(allPoints, EnemyType.Ranged);
            List<SpawnPointData> bossPoints = FilterPoints(allPoints, EnemyType.Boss);

            for (int i = 0; i < wave.meleeCount; i++)
            {
                SpawnEnemy(levelData.meleePrefabs, meleePoints);
                yield return new WaitForSeconds(0.3f);
            }

            for (int i = 0; i < wave.rangedCount; i++)
            {
                SpawnEnemy(levelData.rangedPrefabs, rangedPoints);
                yield return new WaitForSeconds(0.3f);
            }

            if (wave.spawnBoss)
            {
                SpawnEnemy(levelData.bossPrefabs, bossPoints);
                yield return new WaitForSeconds(0.3f);
            }
        }

        private List<SpawnPointData> FilterPoints(SpawnPointData[] all, EnemyType type)
        {
            var result = new List<SpawnPointData>();
            foreach (var p in all)
            {
                if (p.Type == type) result.Add(p);
            }
            return result;
        }

        private void SpawnEnemy(GameObject[] prefabs, List<SpawnPointData> points)
        {
            if (prefabs == null || prefabs.Length == 0 || points == null || points.Count == 0)
                return;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            SpawnPointData point = points[Random.Range(0, points.Count)];

            GameObject enemyObj = Instantiate(prefab, point.Position, Quaternion.identity);
            _livingEnemyCount++;

            AgentHealth health = enemyObj.GetComponentInChildren<AgentHealth>();
            if (health != null)
                health.OnDead += OnEnemyDead;
        }

        private void OnEnemyDead()
        {
            _livingEnemyCount = Mathf.Max(0, _livingEnemyCount - 1);
        }
    }
}
