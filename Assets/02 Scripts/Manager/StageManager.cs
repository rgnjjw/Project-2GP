using System.Collections;
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
        [SerializeField] private GameObject[] arrows;
        
        [Tooltip("무한 순환할 맵 목록. 한 맵 클리어 시 직전 맵을 제외한 랜덤 맵으로 교체된다.")]
        [SerializeField] private MapDataSO[] maps;

        [Tooltip("마지막 적 처치 후 맵을 내리기까지의 유예(초). 사망 연출이 끝날 시간을 준다.")]
        [SerializeField] private float clearGraceDelay = 1.5f;

        public int CurrentLevel { get; private set; } = 1;

        // 이 맵의 웨이브를 모두 클리어하고 맵이 다 내려간 상태인지.
        // 상점 문은 이 값이 true일 때만 열린다.
        public bool IsStageCleared { get; private set; }

        private MapDataSO _currentMap;

        protected override void Awake()
        {
            base.Awake();
            mapGenerator.OnGenerateComplete += OnMapGenerated;
            mapGenerator.OnDestroyComplete += OnMapDestroyed;
            waveManager.OnAllWavesCleared += OnAllWavesCleared;
            foreach (var arrow in arrows)
            {
                arrow.SetActive(false);
            }
        }

        private void Start()
        {
            if (maps != null && maps.Length > 0)
                StartStage(PickNextMap());
        }

        public void StartStage(MapDataSO mapData)
        {
            IsStageCleared = false; // 새 스테이지 시작 → 클리어 상태 해제(상점 문 닫힘)
            foreach (var arrow in arrows)
            {
                arrow.SetActive(false);
            }
            _currentMap = mapData;
            waveManager.SetMapData(mapData);
            mapGenerator.StartGenerate(mapData);
        }

        // 맵 생성 연출이 끝나면 웨이브 시작
        private void OnMapGenerated()
        {
            waveManager.StartWaves(CurrentLevel);
        }

        // 이 맵의 모든 웨이브 클리어 → 난이도 한 단계 올리고, 사망 연출 후 맵 제거
        private void OnAllWavesCleared()
        {
            CurrentLevel++;
            StartCoroutine(DescendAfterDelay());
        }

        // 마지막 적의 사망 연출이 끝날 시간을 준 뒤 맵을 내린다(시체가 남은 채 갑자기 내려가지 않도록).
        private IEnumerator DescendAfterDelay()
        {
            yield return new WaitForSeconds(clearGraceDelay);
            mapGenerator.StartDestroy();
        }

        // 맵 제거 연출이 끝나면 클리어 상태로 전환한다.
        // 다음 맵은 자동 생성하지 않는다 → 상점에서 스테이지 버튼(StartStage)으로 진행한다.
        private void OnMapDestroyed()
        {
            IsStageCleared = true;
            foreach (var arrow in arrows)
            {
                arrow.SetActive(true);
            }
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
