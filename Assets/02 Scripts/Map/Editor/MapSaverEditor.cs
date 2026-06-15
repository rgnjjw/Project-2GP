using System.Collections.Generic;
using _02_Scripts.Enemy;
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

            EditorGUILayout.Space();

            MapRoot root = (MapRoot)target;

            GUI.enabled = root.TargetData != null;
            if (GUILayout.Button("💾 Save Map", GUILayout.Height(30)))
                SaveMap(root);
            GUI.enabled = true;

            if (root.TargetData == null)
                EditorGUILayout.HelpBox("TargetData를 할당하세요.", MessageType.Warning);
        }

        private void SaveMap(MapRoot root)
        {
            MapDataSO data = root.TargetData;
            Undo.RecordObject(data, "Save Map");

            var objects = new List<MapObjectData>();
            foreach (Transform child in root.transform)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (prefab == null)
                {
                    Debug.LogWarning($"[MapSaver] 프리팹이 아닌 오브젝트 스킵: {child.name}");
                    continue;
                }

                objects.Add(new MapObjectData
                {
                    Prefab = prefab,
                    Position = child.position,
                    Rotation = child.rotation
                });
            }

            var spawnPoints = new List<SpawnPointData>();
            EnemySpawnPoint[] pointComponents = FindObjectsByType<EnemySpawnPoint>(FindObjectsSortMode.None);
            foreach (var sp in pointComponents)
            {
                spawnPoints.Add(new SpawnPointData
                {
                    Type = sp.Type,
                    Position = sp.transform.position
                });
            }

            data.Objects = objects.ToArray();
            data.SpawnPoints = spawnPoints.ToArray();

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();

            Debug.Log($"[MapSaver] 저장 완료 — 오브젝트: {objects.Count}개, 스폰포인트: {spawnPoints.Count}개");
        }
    }
}
