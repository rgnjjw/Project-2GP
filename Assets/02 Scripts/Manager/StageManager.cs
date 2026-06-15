using System;
using _02_Scripts.Core.Utility;
using _02_Scripts.Map;
using UnityEngine;

namespace _02_Scripts.Manager
{
    public class StageManager : MonoSingleton<StageManager>
    {
        [SerializeField] private MapGenerator mapGenerator;
        [SerializeField] private WaveManager waveManager;
        
        [SerializeField] private MapDataSO testMapData; // 임시

        private void Start()
        {
            if (testMapData != null)
                StartStage(testMapData);
        }
        public int CurrentLevel { get; private set; } = 1;
        
        protected override void Awake()
        {
            base.Awake();
            mapGenerator.OnGenerateComplete += OnMapGenerated;
            waveManager.OnAllWavesCleared += OnAllWavesCleared;
        }

        public void StartStage(MapDataSO mapData)
        {
            waveManager.SetMapData(mapData);
            mapGenerator.StartGenerate(mapData);
        }

        private void OnMapGenerated()
        {
            waveManager.StartWaves(CurrentLevel);
        }

        private void OnAllWavesCleared()
        {
            mapGenerator.StartDestroy();
            CurrentLevel++;
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
