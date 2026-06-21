using _02_Scripts.Manager;
using UnityEngine;

namespace _02_Scripts.Map
{
    // 맵 생성 트리거. 플레이어가 이 트리거 콜라이더에 닿으면 랜덤 맵으로 다음 스테이지를 생성한다.
    // StageManager.StartNextStage()는 스테이지 클리어 상태일 때만 동작하므로,
    // 전투 중에 닿아도 무시되고 클리어 후 출구로 들어갈 때만 작동한다.
    [RequireComponent(typeof(Collider))]
    public class MapGenerateTrigger : MonoBehaviour
    {
        private void Reset()
        {
            // 에디터에서 컴포넌트 붙일 때 콜라이더를 자동으로 트리거로 설정.
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            if (StageManager.Instance == null) return;

            StageManager.Instance.StartNextStage();
        }
    }
}
