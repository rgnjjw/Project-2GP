using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace _02_Scripts.Map
{
    public class MapGenerator : MonoBehaviour
    {
        [SerializeField] private MapDataSO data;
        [SerializeField] private float generateDuration = 0.5f;
        [SerializeField] private float waitBetweenObjects = 0.1f;
        [SerializeField] private Ease ease = Ease.OutBack;


        private void Start()
        {
            StartGenerate();
        }

        public event Action OnGenerateComplete;

        public void StartGenerate() => StartCoroutine(GenerateMap());

        private IEnumerator GenerateMap()
        {
            foreach (var objData in data.Objects)
            {
                if (objData.Prefab == null) continue;

                Vector3 spawnPos = new Vector3(objData.Position.x, 0f, objData.Position.z);
                GameObject obj = Instantiate(objData.Prefab, spawnPos, objData.Rotation);

                obj.transform.DOMoveY(objData.Position.y, generateDuration).SetEase(ease);

                yield return new WaitForSeconds(waitBetweenObjects);
            }

            OnGenerateComplete?.Invoke();
        }
    }
}