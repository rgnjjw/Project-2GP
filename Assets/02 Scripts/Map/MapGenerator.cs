using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private float generateDuration = 0.5f;
        [SerializeField] private float waitTime = 0.2f;
        [SerializeField] private Ease ease = Ease.Linear;

        private readonly List<GameObject> _currentMapObjectList = new();
        private Sequence _activeSequence;

        public IEnumerator GenerateMap(MapDataSO mapData)
        {
            if (_currentMapObjectList.Count > 0)
                yield return StartCoroutine(DestroyMap());

            _activeSequence?.Kill();
            _activeSequence = DOTween.Sequence();

            float delay = 0f;

            foreach (var mapObject in mapData.MapObjectList)
            {
                GameObject obj = Instantiate(mapObject.prefab, transform);

                obj.transform.localEulerAngles = mapObject.rotation;
                obj.transform.localScale = mapObject.scale;
                obj.transform.localPosition = mapObject.spawnPosition;

                _currentMapObjectList.Add(obj);

                _activeSequence.Insert(delay, obj.transform.DOLocalMove(mapObject.position, generateDuration).SetEase(ease));

                //꼿는 시간만 다르게 하기
                delay += waitTime;
            }

            yield return _activeSequence.WaitForCompletion();
            //트윈이 끝날때까지 대기를 한다 WaitForCompletion() 이함수
        }

        public IEnumerator DestroyMap()
        {
            _activeSequence?.Kill();
            _activeSequence = DOTween.Sequence();

            float delay = 0f;

            foreach (var mapObject in _currentMapObjectList)
            {
                if (mapObject == null) continue;

                MapObject info = mapObject.GetComponent<MapObject>();

                Vector3 targetPos = new Vector3(
                    mapObject.transform.localPosition.x,
                    info != null ? info.spawnPositionY : mapObject.transform.localPosition.y,
                    mapObject.transform.localPosition.z);

                _activeSequence.Insert(delay, mapObject.transform.DOLocalMove(targetPos, generateDuration).SetEase(ease));

                delay += waitTime;
            }

            yield return _activeSequence.WaitForCompletion();//트윈 끝날떄까지 기다리기

            foreach (var mapObject in _currentMapObjectList)
                if (mapObject != null)
                    Destroy(mapObject);

            _currentMapObjectList.Clear();
        }

        private void OnDestroy()
        {
            _activeSequence?.Kill();
        }
    }
}