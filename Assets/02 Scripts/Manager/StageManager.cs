using _02_Scripts.Core.Utility;
using _02_Scripts.Map;
using UnityEngine;

namespace _02_Scripts.Manager
{
    public class StageManager : MonoSingleton<StageManager>
    {
        [SerializeField] private GameObject mapSelectorRoot;
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private WaveManager waveManager;

        public int CurrentLevel { get; private set; } = 1;

        protected override void Awake()
        {
            base.Awake();
            mapGenerator.OnGenerateComplete += OnMapGenerated;
            waveManager.OnAllWavesCleared += OnAllWavesCleared;
        }

        public void StartStage(MapDataSO mapData)
        {
            mapSelectorRoot.SetActive(false);
            waveManager.SetMapData(mapData);
            mapGenerator.StartGenerate(mapData);
        }

        private void OnMapGenerated()
        {
            waveManager.StartWaves(CurrentLevel);
        }

        private void OnAllWavesCleared()
        {
            CurrentLevel++;
            mapSelectorRoot.SetActive(true);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (mapGenerator != null)
                mapGenerator.OnGenerateComplete -= OnMapGenerated;
            if (waveManager != null)
                waveManager.OnAllWavesCleared -= OnAllWavesCleared;
        }
    }
}
