using UnityEditor;
using UnityEngine;

namespace _02_Scripts.Core.Utility
{
    public abstract class EnableMeshReadWrite
    {
        [MenuItem("Tools/Enable Read-Write on All Meshes")]
        static void Enable()
        {
            string[] guids = AssetDatabase.FindAssets("t:Model");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

                if (importer == null) continue;
                if (importer.isReadable) continue;

                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            Debug.Log("완료");
        }
    }
}