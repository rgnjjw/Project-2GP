// using _02_Scripts.Manager;
// using UnityEngine;
//
// namespace _02_Scripts.Map.UI
// {
//     public class StageButtonUI : MonoBehaviour
//     {
//         [SerializeField] private MapDataSO mapData;
//         public void Click()
//         {
//             // StageButtonUI 대신 StageManager에서 실행
//             StageManager.Instance.StartCoroutine(StageManager.Instance.StartStage(mapData));
//         }
//     }
// }