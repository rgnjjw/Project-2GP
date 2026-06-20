using System;
using System.Collections;
using System.Collections.Generic;
using _02_Scripts.Agent;
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

        // 타입별 스폰 위치 캐시(맵 단위로 한 번만 필터)
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
            _waveIndex = 0;
            StartCoroutine(RunWaves(currentLevel));
        }

        private IEnumerator RunWaves(int currentLevel)
        {
            while (_waveIndex < _mapData.Waves.Length)
            {
                yield return StartCoroutine(SpawnWave(_mapData.Waves[_waveIndex], currentLevel));
                // 살아있는 적이 전부(소환된 적 포함) 죽을 때까지 대기
                yield return new WaitUntil(() => _02_Scripts.Enemy.Enemy.AliveCount <= 0);
                _waveIndex++;
            }
            OnAllWavesCleared?.Invoke();
        }

        private IEnumerator SpawnWave(WaveDataSO wave, int currentLevel)
        {
            EnemyLevelData levelData = enemyTable.GetData(currentLevel);
            if (levelData == null) yield break;

            // 레벨로 무한 증가하는 기준 수 × 이 웨이브의 상대 배수 = 이 웨이브의 총 적 수
            int total = Mathf.Max(0, Mathf.RoundToInt(
                (baseEnemies + (currentLevel - 1) * enemiesPerLevel) * wave.sizeMultiplier));

            // 총 적 수를 맵의 근거리:원거리:보스 가중치로 분배(보스도 총 수에 포함)
            SplitByWeight(total, _mapData.meleeWeight, _mapData.rangedWeight, _mapData.bossWeight,
                out int meleeCount, out int rangedCount, out int bossCount);

            // 웨이브 시작 시 한 번에(아주 짧은 간격으로) 전부 생성
            for (int i = 0; i < meleeCount; i++)
            {
                SpawnEnemy(levelData.meleePrefabs, _meleePoints);
                if (spawnStagger > 0f) yield return new WaitForSeconds(spawnStagger);
            }

            for (int i = 0; i < rangedCount; i++)
            {
                SpawnEnemy(levelData.rangedPrefabs, _rangedPoints);
                if (spawnStagger > 0f) yield return new WaitForSeconds(spawnStagger);
            }

            for (int i = 0; i < bossCount; i++)
            {
                SpawnEnemy(levelData.bossPrefabs, _bossPoints);
                if (spawnStagger > 0f) yield return new WaitForSeconds(spawnStagger);
            }
        }

        // 총 수를 근거리:원거리:보스 가중치로 나눈다(반올림 오차는 근거리에 흡수, 합은 항상 total).
        private static void SplitByWeight(int total, float meleeWeight, float rangedWeight, float bossWeight,
            out int meleeCount, out int rangedCount, out int bossCount)
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

        private void CachePoints()
        {
            _meleePoints.Clear();
            _rangedPoints.Clear();
            _bossPoints.Clear();

            if (_mapData?.SpawnPoints == null) return;

            foreach (var p in _mapData.SpawnPoints)
            {
                switch (p.Type)
                {
                    case EnemyType.Melee: _meleePoints.Add(p); break;
                    case EnemyType.Ranged: _rangedPoints.Add(p); break;
                    case EnemyType.Boss: _bossPoints.Add(p); break;
                }
            }
        }

        // 스폰 위치를 중심으로 반경 안 랜덤 지점을 골라 '항상' NavMesh 위로 스냅한다.
        // NavMesh는 에이전트 반경만큼 벽에서 떨어져 구워지므로, NavMesh 위 지점은 벽에 끼지 않는다.
        private Vector3 GetJitteredPosition(Vector3 center)
        {
            Vector3 candidate = center;
            if (spawnJitterRadius > 0f)
            {
                Vector2 offset = Random.insideUnitCircle * spawnJitterRadius;
                candidate = center + new Vector3(offset.x, 0f, offset.y);
            }

            // 1차: jitter 지점 주변에서 NavMesh 표면 탐색
            float range = Mathf.Max(spawnJitterRadius, 1f) + 1f;
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, range, NavMesh.AllAreas))
                return hit.position;

            // 2차: 원본 위치 주변을 넓게 재탐색(디자이너가 위치를 벽/허공 근처에 둔 경우 보정)
            if (NavMesh.SamplePosition(center, out NavMeshHit hit2, 5f, NavMesh.AllAreas))
                return hit2.position;

            // 그래도 못 찾으면 NavMesh가 너무 멀다는 뜻 → 경고하고 원본 위치 사용
            Debug.LogWarning($"[WaveManager] 스폰 위치 근처에서 NavMesh를 못 찾음. 위치를 점검하세요: {center}");
            return center;
        }

        // 주어진 타입 위치들 중 하나를 랜덤으로 골라(재사용) 스폰.
        private void SpawnEnemy(GameObject[] prefabs, List<SpawnPointData> points)
        {
            if (prefabs == null || prefabs.Length == 0 || points == null || points.Count == 0)
                return;

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            SpawnPointData point = points[Random.Range(0, points.Count)];
            Vector3 spawnPos = GetJitteredPosition(point.Position);

            // 적은 자신의 Awake에서 Enemy.AliveEnemies 레지스트리에 스스로 등록한다(소환된 적도 동일).
            // 따라서 여기서 별도로 카운트하지 않고, 클리어 판정은 Enemy.AliveCount로 한다.
            GameObject enemyObj = Instantiate(prefab, spawnPos, Quaternion.identity);

            if (enemyObj.GetComponentInChildren<_02_Scripts.Enemy.Enemy>() == null)
                Debug.LogWarning($"[WaveManager] 스폰된 프리팹에 Enemy 컴포넌트 없음 → 클리어 카운트에 안 잡힘: {enemyObj.name}");
        }
    }
}
