using _02_Scripts.Core.Detect;
using UnityEditor;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill.Editor
{
    [CustomEditor(typeof(EnemyDataSO))]
    public class EnemyDataSOEditor : UnityEditor.Editor
    {
        private enum DetectType { None, Sphere, Cone, Box }
        private DetectType _chaseRangeType;

        private void OnEnable()
        {
            var data = (EnemyDataSO)target;
            _chaseRangeType = GetDetectType(data.ChaseRange);
        }

        public override void OnInspectorGUI()
        {
            var data = (EnemyDataSO)target;
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, "<ChaseRange>k__BackingField", "<EnemySkills>k__BackingField");

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Chase Range", EditorStyles.boldLabel);
            var newType = (DetectType)EditorGUILayout.EnumPopup("Type", _chaseRangeType);
            if (newType != _chaseRangeType)
            {
                _chaseRangeType = newType;
                data.ChaseRange = CreateDetection(newType);
                EditorUtility.SetDirty(target);
            }
            DrawDetectionFields(data.ChaseRange);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("<EnemySkills>k__BackingField"), new GUIContent("Enemy Skills"), true);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawDetectionFields(AbstractDetection detection)
        {
            if (detection == null) return;

            EditorGUI.indentLevel++;

            var type = detection.GetType();
            var fields = type.GetFields(
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            bool changed = false;
            foreach (var field in fields)
            {
                var attrs = field.GetCustomAttributes(typeof(SerializeField), true);
                if (attrs.Length == 0 && !field.IsPublic) continue;

                object value = field.GetValue(detection);

                if (field.FieldType == typeof(float))
                {
                    float newVal = EditorGUILayout.FloatField(field.Name, (float)value);
                    if (!newVal.Equals(value)) { field.SetValue(detection, newVal); changed = true; }
                }
                else if (field.FieldType == typeof(Vector3))
                {
                    Vector3 newVal = EditorGUILayout.Vector3Field(field.Name, (Vector3)value);
                    if (newVal != (Vector3)value) { field.SetValue(detection, newVal); changed = true; }
                }
                else if (field.FieldType == typeof(LayerMask))
                {
                    LayerMask mask = (LayerMask)value;
                    int layerIndex = mask.value == 0 ? 0 : Mathf.RoundToInt(Mathf.Log(mask.value, 2));
                    int newIndex = EditorGUILayout.LayerField(field.Name, layerIndex);
                    LayerMask newMask = 1 << newIndex;
                    if (newMask.value != mask.value) { field.SetValue(detection, newMask); changed = true; }
                }
                else if (field.FieldType == typeof(Color))
                {
                    Color newVal = EditorGUILayout.ColorField(field.Name, (Color)value);
                    if (newVal != (Color)value) { field.SetValue(detection, newVal); changed = true; }
                }
            }

            if (changed)
                EditorUtility.SetDirty(target);

            EditorGUI.indentLevel--;
        }

        private AbstractDetection CreateDetection(DetectType type) => type switch
        {
            DetectType.Sphere => new SphereDetect(),
            DetectType.Cone => new ConeDetect(),
            DetectType.Box => new BoxDetect(),
            _ => null
        };

        private DetectType GetDetectType(AbstractDetection detection) => detection switch
        {
            SphereDetect => DetectType.Sphere,
            ConeDetect => DetectType.Cone,
            BoxDetect => DetectType.Box,
            _ => DetectType.None
        };
    }
}