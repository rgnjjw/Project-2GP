using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private List<MapDataSO> mapDataSOList;
        [SerializeField] private float generateDuration = 0.5f;
        [SerializeField] private float waitTime = 0.2f;
        [SerializeField] private Ease ease = Ease.Linear;
        [SerializeField] private float initPosY;
        private readonly List<GameObject> _currentMapObjectList =  new List<GameObject>();

        public void GenerateMap(int mapIndex)
        {
            if (_currentMapObjectList.Count > 0)
                DestroyMap();
            
            StartCoroutine(GenerateMapRoutine(mapIndex));
        }

        private IEnumerator GenerateMapRoutine(int mapIndex)
        {
            foreach (var mapObject in mapDataSOList[mapIndex].MapObjectList)
            {
                GameObject obj = Instantiate(mapObject.prefab, transform);
                obj.transform.localEulerAngles = mapObject.rotation;
                obj.transform.localScale = mapObject.scale;
                
                Vector3 initPos = new Vector3(mapObject.position.x, initPosY, mapObject.position.z);
                obj.transform.localPosition = initPos;

                obj.transform.DOLocalMoveY(mapObject.position.y, generateDuration).SetEase(ease);
                yield return new WaitForSeconds(waitTime);
                _currentMapObjectList.Add(obj); 
            }
        }

        //test
        [ContextMenu("Generate Map")]
        public void MapGenerateTest()
        {
            StartCoroutine(GenerateMapRoutine(0));
        }

        [ContextMenu("Destroy Map")]
        public void DestroyMap()
        {
            foreach (var mapObject in _currentMapObjectList)
            {
                mapObject.transform.DOLocalMoveY(initPosY, generateDuration).SetEase(ease)
                    .OnComplete(() =>
                    {
                        _currentMapObjectList.Remove(mapObject);
                        Destroy(mapObject);
                    });
            }
        }
    }
}