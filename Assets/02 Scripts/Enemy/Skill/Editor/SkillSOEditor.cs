using _02_Scripts.Core.Detect;
using UnityEditor;
using UnityEngine;

namespace _02_Scripts.Enemy.Skill.Editor
{
    [CustomEditor(typeof(SkillSO), true)]
    public class SkillSOEditor : UnityEditor.Editor
    {
        private enum DetectType { None, Sphere, Cone, Box }

        private DetectType _targetFinderType;
        private DetectType _damageAreaType;

        private void OnEnable()
        {
            var skill = (SkillSO)target;
            _targetFinderType = GetDetectType(skill.TargetFinder);
            _damageAreaType = GetDetectType(skill.DamageAreaDetection);
        }

        public override void OnInspectorGUI()
        {
            var skill = (SkillSO)target;
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("<LandingAnimParam>k__BackingField"), new GUIContent("Anim Param"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("<Cooldown>k__BackingField"), new GUIContent("Cooldown"));

            EditorGUILayout.Space();

            var iterator = serializedObject.GetIterator();
            iterator.NextVisible(true);
            while (iterator.NextVisible(false))
            {
                if (iterator.name is "<LandingAnimParam>k__BackingField"
                    or "<Cooldown>k__BackingField"
                    or "DamageAreaDetection"
                    or "TargetFinder"
                    or "m_Script") continue;

                EditorGUILayout.PropertyField(iterator, true);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Target Finder", EditorStyles.boldLabel);
            var newTargetType = (DetectType)EditorGUILayout.EnumPopup("Type", _targetFinderType);
            if (newTargetType != _targetFinderType)
            {
                _targetFinderType = newTargetType;
                skill.TargetFinder = CreateDetection(newTargetType);
                EditorUtility.SetDirty(target);
            }
            DrawDetectionFields(skill.TargetFinder);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Damage Area Detection", EditorStyles.boldLabel);
            var newDamageType = (DetectType)EditorGUILayout.EnumPopup("Type", _damageAreaType);
            if (newDamageType != _damageAreaType)
            {
                _damageAreaType = newDamageType;
                skill.DamageAreaDetection = CreateDetection(newDamageType);
                EditorUtility.SetDirty(target);
            }
            DrawDetectionFields(skill.DamageAreaDetection);

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