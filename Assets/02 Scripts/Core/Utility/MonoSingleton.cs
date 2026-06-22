using UnityEngine;

namespace _02_Scripts.Core.Utility
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindFirstObjectByType<T>();
                // 못 찾으면 null 반환. (예전엔 빈 GameObject를 즉석 생성했는데,
                //  인스펙터 데이터가 없는 매니저가 만들어져 NullReference의 원인이 되었음 → 제거)
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            // 표준 싱글톤: 먼저 깬 인스턴스가 주인, 이후 중복은 스스로 파괴.
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}
