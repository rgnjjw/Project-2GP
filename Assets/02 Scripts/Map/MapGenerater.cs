using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.AI.Navigation;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private float generateDuration = 0.5f;
        [SerializeField] private float waitBetweenObjects = 0.1f;
        [SerializeField] private Ease ease = Ease.OutBack;
        [SerializeField] private NavMeshSurface navMeshSurface;
        private MapDataSO _data;

        public event Action OnGenerateComplete;
        public event Action OnDestroyComplete;

        private readonly List<(GameObject obj, float spawnY)> _spawnedObjects = new();

        [ContextMenu("맵 생성")]
        public void StartGenerate() => StartCoroutine(GenerateMap());

        public void StartGenerate(MapDataSO overrideData)
        {
            _data = overrideData;
            StartCoroutine(GenerateMap());
        }

        [ContextMenu("맵 제거")]
        public void StartDestroy() => StartCoroutine(DestroyMap());

        private IEnumerator GenerateMap()
        {
            _spawnedObjects.Clear();

            foreach (var objData in _data.Objects)
            {
                if (objData.Prefab == null) continue;

                float spawnY = objData.Position.y - 20f;
                MapSpawnOffset offset = objData.Prefab.GetComponent<MapSpawnOffset>();
                if (offset != null)
                    spawnY = offset.SpawnOffsetY;

                Vector3 spawnPos = new Vector3(objData.Position.x, spawnY, objData.Position.z);
                GameObject obj = Instantiate(objData.Prefab, spawnPos, objData.Rotation);

                _spawnedObjects.Add((obj, spawnY));

                obj.transform.DOMoveY(objData.Position.y, generateDuration).SetEase(ease);

                yield return new WaitForSeconds(waitBetweenObjects);
            }

            yield return new WaitForSeconds(generateDuration);
            navMeshSurface.BuildNavMesh();
            OnGenerateComplete?.Invoke();
        }

        private IEnumerator DestroyMap()
        {
            foreach (var (obj, spawnY) in _spawnedObjects)
            {
                if (obj == null) continue;
                obj.transform.DOMoveY(spawnY, generateDuration).SetEase(ease);
                yield return new WaitForSeconds(waitBetweenObjects);
            }

            yield return new WaitForSeconds(generateDuration);

            foreach (var (obj, _) in _spawnedObjects)
            {
                if (obj != null) Destroy(obj);
            }

            _spawnedObjects.Clear();
            OnDestroyComplete?.Invoke();
        }
    }
}