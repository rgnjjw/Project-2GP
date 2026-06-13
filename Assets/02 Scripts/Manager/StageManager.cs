// using System.Collections;
// using _02_Scripts.Chip.Card;
// using _02_Scripts.Core.Utility;
// using _02_Scripts.Map;
// using Unity.AI.Navigation;
// using UnityEngine;
//
// namespace _02_Scripts.Manager
// {
//     public class StageManager : MonoSingleton<StageManager>
//     {
//         [SerializeField] private GameObject mapSelector;
//         [SerializeField] private MapGenerator mapGenerator;
//         [SerializeField] private NavMeshSurface navMeshSurface;
//         [field: SerializeField] public ChipCardUI ChipCardUI { get; private set; }
//
//         public int EnemyCount
//         {
//             get => _enemyCount;
//             set
//             {
//                 _enemyCount = value;
//                 if (_enemyCount <= 0)
//                     StartCoroutine(EndStage());
//             }
//         }
//         private int _enemyCount;
//
//         public int CurrentStage { get; private set; }
//
//         public IEnumerator StartStage(MapDataSO mapData)
//         {
//             _enemyCount = 0;
//             mapSelector.SetActive(false);
//             mapGenerator.OnGenerateComplete += OnMapGenerated;
//             mapGenerator.StartGenerate();
//             yield return new WaitUntil(() => _mapGenerated);
//             _mapGenerated = false;
//             navMeshSurface.BuildNavMesh();
//         }
//
//         private bool _mapGenerated;
//         private void OnMapGenerated()
//         {
//             mapGenerator.OnGenerateComplete -= OnMapGenerated;
//             _mapGenerated = true;
//         }
//
//         public IEnumerator EndStage()
//         {
//             mapSelector.SetActive(true);
//             CurrentStage++;
//         }
//     }
// }