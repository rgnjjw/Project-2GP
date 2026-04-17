using _02_Scripts.Map;
using UnityEditor;
using UnityEngine;

namespace _02_Scripts.Editor
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
            Undo.RecordObject(mapDataSO, "Save Map Data");//수정전에 저장 언도 기능 쓰려고 저장하기
            mapDataSO.MapObjectList.Clear();//덮어 씌우기 위해 비워주기

            foreach (Transform child in mapRoot.transform)
            {
                GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (prefab == null)
                {
                    Debug.LogError("프리팹이 아닌 오브젝트가 있습니다");
                    return;     
                }
                
                mapDataSO.MapObjectList.Add(new MapObjectData
                {
                    prefab = prefab,
                    position = child.localPosition,
                    rotation = child.localEulerAngles,
                    scale = child.localScale
                });
            }
            
            EditorUtility.SetDirty(mapDataSO);//유니티한테 저장 필요하다고 알려주기
            AssetDatabase.SaveAssets();//저장
        }
    }
}