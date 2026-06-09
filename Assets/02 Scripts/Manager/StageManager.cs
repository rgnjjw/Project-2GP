using System.Collections;
using _02_Scripts.Chip.Card;
using _02_Scripts.Core.Utility;
using _02_Scripts.Map;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace _02_Scripts.Manager
{
    public class StageManager : MonoSingleton<StageManager>
    {
        [SerializeField] private GameObject mapSelector;//맵 중앙 조작 패널
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private DoorTrigger doorTrigger; //(임시) 스테이지 고르는 문 닫기위해서
        [SerializeField] private NavMeshSurface navMeshSurface;
        [field: SerializeField] public ChipCardUI ChipCardUI {get; private set; }//임시 (나중에 삭제 할 것)

        public int EnemyCount {get => _enemyCount;//임시
            set
            {
                _enemyCount = value;
                if (_enemyCount <= 0)
                {
                    StartCoroutine(EndStage());
                }
            }
        }
        private int _enemyCount;
        
        public int CurrentStage { get; private set; }

        public IEnumerator StartStage(MapDataSO mapData)
        {
            _enemyCount = 0;
            mapSelector.SetActive(false);
            yield return StartCoroutine(mapGenerator.GenerateMap(mapData));
            navMeshSurface.BuildNavMesh();
            foreach (var enemySpawnData in mapData.EnemySpawnData)
            {
                Vector3 spawnPos = enemySpawnData.enemySpawnPoint;
                if (NavMesh.SamplePosition(spawnPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
                    spawnPos = hit.position;
        
                GameObject enemy = Instantiate(enemySpawnData.enemyPrefab, spawnPos, Quaternion.identity);
                _enemyCount++;
            }
        }

        public IEnumerator EndStage()
        {
            yield return StartCoroutine(mapGenerator.DestroyMap()); //이러면 destroy맵이 끝나야 다음꺼 실행
            mapSelector.SetActive(true);
            doorTrigger.Close();
            CurrentStage++;
        }
    }
}