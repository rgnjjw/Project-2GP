// MapSaverEditor.cs
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _02_Scripts.Map.Editor
{
    [CustomEditor(typeof(MapRoot))]
    public class MapSaverEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MapRoot mapRoot = (MapRoot)target;

            if (GUILayout.Button("💾SaveMap"))
            {
                SaveMap(mapRoot);
            }
        }

        private void SaveMap(MapRoot mapRoot)
        {
            MapDataSO mapDataSO = mapRoot.TargetMapDataSO;
            Undo.RecordObject(mapDataSO, "Save Map DataSO");

            var mapObjectList = new List<MapObjectData>();

            foreach (Transform child in mapRoot.transform)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);

                if (prefab == null)
                {
                    Debug.LogError("프리팹이 아닌 오브젝트가 있습니다");
                    return;
                }

                MapObject info = child.GetComponent<MapObject>();

                Vector3 spawnPos = new Vector3(child.localPosition.x, info != null ? info.spawnPositionY : child.localPosition.y, child.localPosition.z);

                mapObjectList.Add(new MapObjectData
                {
                    prefab = prefab,
                    position = child.localPosition,
                    rotation = child.localEulerAngles,
                    scale = child.localScale,
                    spawnPosition = spawnPos
                });
            }

            mapDataSO.MapObjectList = mapObjectList.ToArray();

            EditorUtility.SetDirty(mapDataSO);
            AssetDatabase.SaveAssets();

            Debug.Log("맵 저장 완료");
        }
    }
}