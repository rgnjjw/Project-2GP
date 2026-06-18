using System.Linq;
using UnityEditor;
using UnityEngine;

namespace _02_Scripts.SceneChange.Editor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneNameProp = property.FindPropertyRelative("sceneName");

            var scenePaths = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            var sceneNames = scenePaths
                .Select(p => System.IO.Path.GetFileNameWithoutExtension(p))
                .ToArray();

            if (sceneNames.Length == 0)
            {
                EditorGUI.LabelField(position, label.text, "빌드 세팅에 씬이 없음");
                return;
            }

            int currentIndex = System.Array.IndexOf(sceneNames, sceneNameProp.stringValue);
            if (currentIndex < 0) currentIndex = 0;

            EditorGUI.BeginProperty(position, label, property);

            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, sceneNames);
            sceneNameProp.stringValue = sceneNames[selectedIndex];

            EditorGUI.EndProperty();
        }
    }
}