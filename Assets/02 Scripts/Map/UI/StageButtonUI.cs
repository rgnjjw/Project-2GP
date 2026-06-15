using _02_Scripts.Manager;
using UnityEngine;

namespace _02_Scripts.Map.UI
{
    public class StageButtonUI : MonoBehaviour
    {
        [SerializeField] private MapDataSO mapData;

        public void Click()
        {
            StageManager.Instance.StartStage(mapData);
        }
    }
}
