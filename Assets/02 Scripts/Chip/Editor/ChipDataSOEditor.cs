using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _02_Scripts.Chip.Editor
{
    [CustomEditor(typeof(ChipDataSO), true)]
    public class ChipDataSOEditor : UnityEditor.Editor
    {
        private bool _levelFoldout = true;
        private bool _statFoldout  = true;
        private bool _extraFoldout = true;

        private Texture2D _headerTex;
        private Texture2D _rowTexA;
        private Texture2D _rowTexB;
        private Texture2D _dividerTex;

        private static readonly Color HeaderBg  = new Color(0.13f, 0.13f, 0.13f);
        private static readonly Color RowA      = new Color(0.22f, 0.22f, 0.22f);
        private static readonly Color RowB      = new Color(0.18f, 0.18f, 0.18f);
        private static readonly Color DividerC  = new Color(0.35f, 0.35f, 0.35f);
        private static readonly Color AccentC   = new Color(0.25f, 0.55f, 1.00f);

        private void OnEnable()
        {
            _headerTex  = MakeTex(HeaderBg);
            _rowTexA    = MakeTex(RowA);
            _rowTexB    = MakeTex(RowB);
            _dividerTex = MakeTex(DividerC);
        }

        private void OnDisable()
        {
            DestroyImmediate(_headerTex);
            DestroyImmediate(_rowTexA);
            DestroyImmediate(_rowTexB);
            DestroyImmediate(_dividerTex);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var chip = (ChipDataSO)target;

            DrawBanner(chip);
            GUILayout.Space(6);
            DrawBasicInfo();
            GUILayout.Space(4);
            DrawLevelTable(chip);
            GUILayout.Space(4);
            DrawStatTable(chip);
            GUILayout.Space(4);
            DrawExtraFields();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBanner(ChipDataSO chip)
        {
            var bannerStyle = new GUIStyle(GUIStyle.none)
            {
                normal = { background = _headerTex },
                padding = new RectOffset(8, 8, 8, 8)
            };

            EditorGUILayout.BeginHorizontal(bannerStyle, GUILayout.Height(52));

            if (chip.Icon != null)
            {
                var tex = AssetPreview.GetAssetPreview(chip.Icon);
                if (tex != null)
                    GUILayout.Label(tex, GUILayout.Width(44), GUILayout.Height(44));
            }
            else
            {
                GUILayout.Label("[no icon]", GUILayout.Width(44));
            }

            GUILayout.Space(8);

            EditorGUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                string.IsNullOrEmpty(chip.Name) ? "(이름 없음)" : chip.Name,
                new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 }
            );
            GUILayout.Label(
                $"ID: {chip.ChipId}   |   {chip.GetType().Name}   |   MaxLv: {chip.MaxLevel}",
                EditorStyles.miniLabel
            );
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawBasicInfo()
        {
            DrawSectionHeader("기본 정보");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(P("<ChipId>k__BackingField"),           new GUIContent("Chip ID"));
            EditorGUILayout.PropertyField(P("<Name>k__BackingField"),              new GUIContent("이름"));
            EditorGUILayout.PropertyField(P("<Icon>k__BackingField"),              new GUIContent("아이콘"));
            EditorGUILayout.PropertyField(P("<MaxLevel>k__BackingField"),          new GUIContent("최대 레벨"));
            EditorGUILayout.PropertyField(P("<LevelNameAndDescData>k__BackingField"), new GUIContent("레벨 이름/설명"), true);
            EditorGUI.indentLevel--;
        }

        private void DrawLevelTable(ChipDataSO chip)
        {
            var nameProp = P("<LevelNameAndDescData>k__BackingField");
            if (nameProp == null || !nameProp.isArray) return;

            _levelFoldout = DrawFoldoutHeader("레벨 텍스트 테이블", _levelFoldout);
            if (!_levelFoldout) return;

            DrawTableHeader(new[] { ("Lv", 30f), ("이름", 120f), ("설명", 0f) });

            for (int i = 0; i < nameProp.arraySize; i++)
            {
                var elem  = nameProp.GetArrayElementAtIndex(i);
                var nProp = elem.FindPropertyRelative("Name");
                var dProp = elem.FindPropertyRelative("Description");

                BeginRow(i);
                LabelCell($"{i + 1}", 30f);
                if (nProp != null) nProp.stringValue = EditorGUILayout.TextField(nProp.stringValue, GUILayout.Width(120));
                if (dProp != null) dProp.stringValue = EditorGUILayout.TextField(dProp.stringValue, GUILayout.ExpandWidth(true));
                EndRow();
            }

            DrawArrayButtons(nameProp);
        }

        private void DrawStatTable(ChipDataSO chip)
        {
            var statProp = serializedObject.FindProperty("<LevelData>k__BackingField");
            if (statProp == null || !statProp.isArray) return;

            var chipType   = chip.GetType();
            var levelProp  = chipType.GetProperty("LevelData", BindingFlags.Public | BindingFlags.Instance);
            if (levelProp == null) return;

            var arr = levelProp.GetValue(chip) as Array;
            if (arr == null || arr.Length == 0)
            {
                if (statProp.arraySize == 0) return;
                arr = null;
            }

            Type elemType = arr != null
                ? arr.GetValue(0)?.GetType()
                : chipType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                          .FirstOrDefaultCustom(t => t.IsClass || t.IsValueType);

            if (elemType == null) return;

            var fields = elemType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            if (fields.Length == 0) return;

            _statFoldout = DrawFoldoutHeader($"스탯 테이블 ({elemType.Name})", _statFoldout);
            if (!_statFoldout) return;

            var headers = new (string label, float width)[fields.Length + 1];
            headers[0] = ("Lv", 30f);
            for (int i = 0; i < fields.Length; i++)
                headers[i + 1] = (fields[i].Name, 0f);

            DrawTableHeader(headers);

            for (int i = 0; i < statProp.arraySize; i++)
            {
                var elem = statProp.GetArrayElementAtIndex(i);
                BeginRow(i);
                LabelCell($"{i + 1}", 30f);

                foreach (var f in fields)
                {
                    var sub = elem.FindPropertyRelative(f.Name);
                    if (sub != null)
                        DrawCompact(sub);
                    else
                        GUILayout.Label("-", GUILayout.ExpandWidth(true));
                }

                EndRow();
            }

            DrawArrayButtons(statProp);
        }

        private void DrawExtraFields()
        {
            var skip = new System.Collections.Generic.HashSet<string>
            {
                "m_Script",
                "<ChipId>k__BackingField",
                "<Name>k__BackingField",
                "<Icon>k__BackingField",
                "<MaxLevel>k__BackingField",
                "<LevelNameAndDescData>k__BackingField",
                "<LevelData>k__BackingField",
            };

            var iter = serializedObject.GetIterator();
            iter.NextVisible(true);
            bool hasExtra = false;
            var check = serializedObject.GetIterator();
            check.NextVisible(true);
            while (check.NextVisible(false))
                if (!skip.Contains(check.name)) { hasExtra = true; break; }

            if (!hasExtra) return;

            _extraFoldout = DrawFoldoutHeader("추가 설정", _extraFoldout);
            if (!_extraFoldout) return;

            EditorGUI.indentLevel++;
            iter.NextVisible(false);
            do
            {
                if (!skip.Contains(iter.name))
                    EditorGUILayout.PropertyField(iter, true);
            }
            while (iter.NextVisible(false));
            EditorGUI.indentLevel--;
        }

        private void DrawSectionHeader(string title)
        {
            GUILayout.Label(title, new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = AccentC }
            });
            DrawDivider();
        }

        private bool DrawFoldoutHeader(string title, bool state)
        {
            GUILayout.Space(2);
            var style = new GUIStyle(EditorStyles.foldoutHeader)
            {
                fontStyle = FontStyle.Bold
            };
            bool result = EditorGUILayout.BeginFoldoutHeaderGroup(state, title, style);
            EditorGUILayout.EndFoldoutHeaderGroup();
            return result;
        }

        private void DrawTableHeader((string label, float width)[] cols)
        {
            var style = new GUIStyle(GUIStyle.none) { normal = { background = _rowTexA } };
            EditorGUILayout.BeginHorizontal(style);
            foreach (var (label, width) in cols)
            {
                if (width > 0)
                    GUILayout.Label(label, EditorStyles.boldLabel, GUILayout.Width(width));
                else
                    GUILayout.Label(label, EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndHorizontal();
            DrawDivider();
        }

        private void BeginRow(int i)
        {
            var tex   = i % 2 == 0 ? _rowTexA : _rowTexB;
            var style = new GUIStyle(GUIStyle.none) { normal = { background = tex } };
            EditorGUILayout.BeginHorizontal(style);
        }

        private void EndRow() => EditorGUILayout.EndHorizontal();

        private void DrawArrayButtons(SerializedProperty prop)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+ 레벨 추가", GUILayout.Width(80))) prop.arraySize++;
            if (GUILayout.Button("- 레벨 제거", GUILayout.Width(80)) && prop.arraySize > 0) prop.arraySize--;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawDivider()
        {
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawPreviewTexture(rect, _dividerTex);
        }

        private void DrawCompact(SerializedProperty p)
        {
            switch (p.propertyType)
            {
                case SerializedPropertyType.Float:
                    p.floatValue = EditorGUILayout.FloatField(p.floatValue, GUILayout.ExpandWidth(true));
                    break;
                case SerializedPropertyType.Integer:
                    p.intValue = EditorGUILayout.IntField(p.intValue, GUILayout.ExpandWidth(true));
                    break;
                case SerializedPropertyType.Boolean:
                    p.boolValue = EditorGUILayout.Toggle(p.boolValue, GUILayout.Width(20));
                    break;
                case SerializedPropertyType.String:
                    p.stringValue = EditorGUILayout.TextField(p.stringValue, GUILayout.ExpandWidth(true));
                    break;
                default:
                    EditorGUILayout.PropertyField(p, GUIContent.none, GUILayout.ExpandWidth(true));
                    break;
            }
        }

        private void LabelCell(string text, float width)
        {
            GUILayout.Label(text, EditorStyles.label, GUILayout.Width(width));
        }

        private SerializedProperty P(string name) => serializedObject.FindProperty(name);

        private static Texture2D MakeTex(Color c)
        {
            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }
    }

    internal static class TypeExtensions
    {
        public static Type FirstOrDefaultCustom(this Type[] types, Func<Type, bool> predicate)
        {
            foreach (var t in types)
                if (predicate(t)) return t;
            return null;
        }
    }
}
