using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private float generateDuration = 0.5f;
        [SerializeField] private float waitTime = 0.2f;
        [SerializeField] private AnimationCurve ease = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float initPosY;
        private readonly List<GameObject> _currentMapObjectList = new();

        public IEnumerator GenerateMap(MapDataSO mapData)
        {
            if (_currentMapObjectList.Count > 0)
                yield return StartCoroutine(DestroyMap());
            
            List<Coroutine> routineList = new List<Coroutine>();
            
            foreach (var mapObject in mapData.MapObjectList)
            {
                GameObject obj = Instantiate(mapObject.prefab, transform);
                obj.transform.localEulerAngles = mapObject.rotation;
                obj.transform.localScale = mapObject.scale;

                Vector3 initPos = new Vector3(mapObject.position.x, initPosY, mapObject.position.z);
                obj.transform.localPosition = initPos;

                _currentMapObjectList.Add(obj);
                routineList.Add(StartCoroutine(MoveLocal(obj, mapObject.position, generateDuration)));
                yield return new WaitForSeconds(waitTime);
            }

            foreach (var routine in routineList)
            {
                yield return routine;
            }
        }

        public IEnumerator DestroyMap()
        {
            List<Coroutine> coroutines = new();

            foreach (var mapObject in _currentMapObjectList)
            {
                if (mapObject == null) continue;
                Vector3 targetPos = new Vector3(
                    mapObject.transform.localPosition.x,
                    initPosY,
                    mapObject.transform.localPosition.z
                );
                coroutines.Add(StartCoroutine(MoveLocal(mapObject, targetPos, generateDuration)));
                yield return new WaitForSeconds(waitTime);
            }

            foreach (var co in coroutines)
                yield return co;

            foreach (var mapObject in _currentMapObjectList)
                Destroy(mapObject);

            _currentMapObjectList.Clear();
        }

        private IEnumerator MoveLocal(GameObject obj, Vector3 targetLocalPos, float duration)
        {
            Vector3 startLocalPos = obj.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float curvedT = ease.Evaluate(t);
                obj.transform.localPosition = Vector3.Lerp(startLocalPos, targetLocalPos, curvedT);
                yield return null;
            }

            obj.transform.localPosition = targetLocalPos;
        }
    }
}