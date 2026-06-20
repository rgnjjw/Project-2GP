using _02_Scripts.Core.Utility;
using _02_Scripts.Map;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _02_Scripts.Manager
{
    public class StageManager : MonoSingleton<StageManager>
    {
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private WaveManager waveManager;

        [Tooltip("무한 순환할 맵 목록. 한 맵 클리어 시 직전 맵을 제외한 랜덤 맵으로 교체된다.")]
        [SerializeField] private MapDataSO[] maps;

        public int CurrentLevel { get; private set; } = 1;

        private MapDataSO _currentMap;

        protected override void Awake()
        {
            base.Awake();
            mapGenerator.OnGenerateComplete += OnMapGenerated;
            mapGenerator.OnDestroyComplete += OnMapDestroyed;
            waveManager.OnAllWavesCleared += OnAllWavesCleared;
        }

        private void Start()
        {
            if (maps != null && maps.Length > 0)
                StartStage(PickNextMap());
        }

        public void StartStage(MapDataSO mapData)
        {
            _currentMap = mapData;
            waveManager.SetMapData(mapData);
            mapGenerator.StartGenerate(mapData);
        }

        // 맵 생성 연출이 끝나면 웨이브 시작
        private void OnMapGenerated()
        {
            waveManager.StartWaves(CurrentLevel);
        }

        // 이 맵의 모든 웨이브 클리어 → 난이도 한 단계 올리고 맵 제거
        private void OnAllWavesCleared()
        {
            CurrentLevel++;
            mapGenerator.StartDestroy();
        }

        // 맵 제거 연출이 끝나면 다음(직전 제외 랜덤) 맵을 솟아오르게
        private void OnMapDestroyed()
        {
            if (maps == null || maps.Length == 0) return;
            StartStage(PickNextMap());
        }

        // 직전 맵을 제외한 랜덤 맵 선택(맵이 하나뿐이면 그대로)
        private MapDataSO PickNextMap()
        {
            if (maps.Length == 1) return maps[0];

            MapDataSO next;
            do
            {
                next = maps[Random.Range(0, maps.Length)];
            }
            while (next == _currentMap);

            return next;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mapGenerator != null)
            {
                mapGenerator.OnGenerateComplete -= OnMapGenerated;
                mapGenerator.OnDestroyComplete -= OnMapDestroyed;
            }
            if (waveManager != null)
                waveManager.OnAllWavesCleared -= OnAllWavesCleared;
        }
    }
}
