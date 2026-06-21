using System;
using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Enemy;
using _02_Scripts.Map;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace _02_Scripts.Manager
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private EnemyTableSO enemyTable;

        [Header("레벨별 적 수")]
        [Tooltip("레벨 1에서의 웨이브당 기준 적 수.")]
        [SerializeField] private int baseEnemies = 6;

        [Tooltip("레벨이 1 오를 때마다 늘어나는 적 수.")]
        [SerializeField] private float enemiesPerLevel = 2f;

        [Tooltip("한 웨이브의 적들을 스폰할 때 적 사이 간격(초). 0이면 한 프레임에 전부.")]
        [SerializeField] private float spawnStagger = 0.07f;

        [Tooltip("같은 스폰 위치에 여러 마리가 겹치지 않도록 주변에 흩뿌리는 반경(m). 0이면 정확히 그 위치.")]
        [SerializeField] private float spawnJitterRadius = 2f;

        public event Action OnAllWavesCleared;

        private MapDataSO _mapData;
        private int _waveIndex;

        private readonly List<SpawnPointData> _meleePoints = new();
        private readonly List<SpawnPointData> _rangedPoints = new();
        private readonly List<SpawnPointData> _bossPoints = new();

        public void SetMapData(MapDataSO mapData)
        {
            _mapData = mapData;
            CachePoints();
        }

        public void StartWaves(int currentLevel)
        {
            StopAllCoroutines();

            if (_mapData == null)
            {
                Debug.LogWarning("[WaveManager] MapData가 없습니다. SetMapData가 먼저 호출되어야 합니다.");
                return;
            }

            if (_mapData.Waves == null || _mapData.Waves.Length == 0)
            {
                Debug.LogWarning("[WaveManager] MapData에 Waves가 없습니다.");
                return;
            }

            _waveIndex = 0;
            StartCoroutine(RunWaves(currentLevel));
        }

        private IEnumerator RunWaves(int currentLevel)
        {
            while (_waveIndex < _mapData.Waves.Length)
            {
                yield return StartCoroutine(SpawnWave(_mapData.Waves[_waveIndex], currentLevel));

                yield return new WaitUntil(() => _02_Scripts.Enemy.Enemy.AliveCount <= 0);

                _waveIndex++;
            }

            OnAllWavesCleared?.Invoke();
        }

        private IEnumerator SpawnWave(WaveDataSO wave, int currentLevel)
        {
            if (wave == null)
            {
                Debug.LogWarning($"[WaveManager] {_waveIndex}번 WaveData가 null입니다.");
                yield break;
            }

            EnemyLevelData levelData = enemyTable.GetData(currentLevel);

            if (levelData == null)
            {
                Debug.LogWarning($"[WaveManager] 현재 레벨에 맞는 EnemyLevelData가 없습니다. Level: {currentLevel}");
                yield break;
            }

            int total = Mathf.Max(0, Mathf.RoundToInt(
                (baseEnemies + (currentLevel - 1) * enemiesPerLevel) * wave.sizeMultiplier));

            SplitByWeight(
                total,
                _mapData.meleeWeight,
                _mapData.rangedWeight,
                _mapData.bossWeight,
                out int meleeCount,
                out int rangedCount,
                out int bossCount
            );

            RedistributeUnavailableCounts(levelData, ref meleeCount, ref rangedCount, ref bossCount);

            Debug.Log(
                $"[WaveManager] Wave:{_waveIndex}, Total:{total}, " +
                $"Melee:{meleeCount}, Ranged:{rangedCount}, Boss:{bossCount}"
            );

            for (int i = 0; i < meleeCount; i++)
            {
                SpawnEnemy(levelData.meleePrefabs, _meleePoints, "Melee");

                if (spawnStagger > 0f)
                    yield return new WaitForSeconds(spawnStagger);
            }

            for (int i = 0; i < rangedCount; i++)
            {
                SpawnEnemy(levelData.rangedPrefabs, _rangedPoints, "Ranged");

                if (spawnStagger > 0f)
                    yield return new WaitForSeconds(spawnStagger);
            }

            for (int i = 0; i < bossCount; i++)
            {
                SpawnEnemy(levelData.bossPrefabs, _bossPoints, "Boss");

                if (spawnStagger > 0f)
                    yield return new WaitForSeconds(spawnStagger);
            }
        }

        private static void SplitByWeight(
            int total,
            float meleeWeight,
            float rangedWeight,
            float bossWeight,
            out int meleeCount,
            out int rangedCount,
            out int bossCount)
        {
            float sum = meleeWeight + rangedWeight + bossWeight;

            if (total <= 0 || sum <= 0f)
            {
                meleeCount = total > 0 ? total : 0;
                rangedCount = 0;
                bossCount = 0;
                return;
            }

            bossCount = Mathf.Clamp(Mathf.RoundToInt(total * (bossWeight / sum)), 0, total);
            rangedCount = Mathf.Clamp(Mathf.RoundToInt(total * (rangedWeight / sum)), 0, total - bossCount);
            meleeCount = total - bossCount - rangedCount;
        }

        private void RedistributeUnavailableCounts(
            EnemyLevelData levelData,
            ref int meleeCount,
            ref int rangedCount,
            ref int bossCount)
        {
            bool canSpawnMelee = CanSpawn(levelData.meleePrefabs, _meleePoints);
            bool canSpawnRanged = CanSpawn(levelData.rangedPrefabs, _rangedPoints);
            bool canSpawnBoss = CanSpawn(levelData.bossPrefabs, _bossPoints);

            int overflowCount = 0;

            if (!canSpawnMelee)
            {
                overflowCount += meleeCount;
                meleeCount = 0;
            }

            if (!canSpawnRanged)
            {
                overflowCount += rangedCount;
                rangedCount = 0;
            }

            if (!canSpawnBoss)
            {
                overflowCount += bossCount;
                bossCount = 0;
            }

            if (overflowCount <= 0)
                return;

            if (canSpawnMelee)
            {
                meleeCount += overflowCount;
                return;
            }

            if (canSpawnRanged)
            {
                rangedCount += overflowCount;
                return;
            }

            if (canSpawnBoss)
            {
                bossCount += overflowCount;
                return;
            }

            Debug.LogWarning("[WaveManager] 스폰 가능한 적 타입이 하나도 없습니다. Enemy Prefab 또는 SpawnPoint 설정을 확인하세요.");
        }

        private bool CanSpawn(GameObject[] prefabs, List<SpawnPointData> points)
        {
            return prefabs != null &&
                   prefabs.Length > 0 &&
                   points != null &&
                   points.Count > 0;
        }

        private void CachePoints()
        {
            _meleePoints.Clear();
            _rangedPoints.Clear();
            _bossPoints.Clear();

            if (_mapData?.SpawnPoints == null)
                return;

            foreach (SpawnPointData point in _mapData.SpawnPoints)
            {
                switch (point.Type)
                {
                    case EnemyType.Melee:
                        _meleePoints.Add(point);
                        break;

                    case EnemyType.Ranged:
                        _rangedPoints.Add(point);
                        break;

                    case EnemyType.Boss:
                        _bossPoints.Add(point);
                        break;
                }
            }
        }

        private Vector3 GetJitteredPosition(Vector3 center)
        {
            Vector3 candidate = center;

            if (spawnJitterRadius > 0f)
            {
                Vector2 offset = Random.insideUnitCircle * spawnJitterRadius;
                candidate = center + new Vector3(offset.x, 0f, offset.y);
            }

            float range = Mathf.Max(spawnJitterRadius, 1f) + 1f;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, range, NavMesh.AllAreas))
                return hit.position;

            if (NavMesh.SamplePosition(center, out NavMeshHit hit2, 5f, NavMesh.AllAreas))
                return hit2.position;

            Debug.LogWarning($"[WaveManager] 스폰 위치 근처에서 NavMesh를 못 찾음. 위치를 점검하세요: {center}");
            return center;
        }

        private void SpawnEnemy(GameObject[] prefabs, List<SpawnPointData> points, string spawnType)
        {
            if (prefabs == null || prefabs.Length == 0)
            {
                Debug.LogWarning($"[WaveManager] {spawnType} 프리팹이 없어서 스폰 스킵됨.");
                return;
            }

            if (points == null || points.Count == 0)
            {
                Debug.LogWarning($"[WaveManager] {spawnType} 스폰 포인트가 없어서 스폰 스킵됨.");
                return;
            }

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];

            if (prefab == null)
            {
                Debug.LogWarning($"[WaveManager] {spawnType} 프리팹 배열 안에 null이 들어있음.");
                return;
            }

            SpawnPointData point = points[Random.Range(0, points.Count)];
            Vector3 spawnPos = GetJitteredPosition(point.Position);

            GameObject enemyObj = Instantiate(prefab, spawnPos, Quaternion.identity);

            if (enemyObj.GetComponentInChildren<_02_Scripts.Enemy.Enemy>() == null)
            {
                Debug.LogWarning($"[WaveManager] 스폰된 프리팹에 Enemy 컴포넌트 없음 → 클리어 카운트에 안 잡힘: {enemyObj.name}");
            }
        }
    }
}