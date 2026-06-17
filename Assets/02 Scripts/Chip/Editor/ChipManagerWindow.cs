using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace _02_Scripts.Chip.Editor
{
    public class ChipManagerWindow : EditorWindow
    {
        [MenuItem("Tools/Chip Manager")]
        public static void Open()
        {
            var win = GetWindow<ChipManagerWindow>("Chip Manager");
            win.minSize = new Vector2(700, 450);
        }

        private List<ChipDataSO> _allChips = new();
        private ChipDataSO _selected;
        private UnityEditor.Editor _selectedEditor;

        private Vector2 _listScroll;
        private Vector2 _detailScroll;

        private string _search = "";
        private string _typeFilter = "전체";
        private List<string> _typeNames = new();

        private Texture2D _sideBgTex;
        private Texture2D _rowHoverTex;
        private Texture2D _rowSelectedTex;
        private Texture2D _rowNormalATex;
        private Texture2D _rowNormalBTex;
        private Texture2D _dividerTex;

        private static readonly Color SideBg       = new Color(0.16f, 0.16f, 0.16f);
        private static readonly Color RowNormalA    = new Color(0.20f, 0.20f, 0.20f);
        private static readonly Color RowNormalB    = new Color(0.17f, 0.17f, 0.17f);
        private static readonly Color RowHover      = new Color(0.26f, 0.30f, 0.38f);
        private static readonly Color RowSelected   = new Color(0.20f, 0.40f, 0.70f);
        private static readonly Color DividerColor  = new Color(0.10f, 0.10f, 0.10f);

        private const float ListWidth   = 260f;
        private const float RowHeight   = 48f;

        private void OnEnable()
        {
            _sideBgTex       = MakeTex(SideBg);
            _rowHoverTex     = MakeTex(RowHover);
            _rowSelectedTex  = MakeTex(RowSelected);
            _rowNormalATex   = MakeTex(RowNormalA);
            _rowNormalBTex   = MakeTex(RowNormalB);
            _dividerTex      = MakeTex(DividerColor);

            Refresh();
        }

        private void OnDisable()
        {
            DestroyImmediate(_sideBgTex);
            DestroyImmediate(_rowHoverTex);
            DestroyImmediate(_rowSelectedTex);
            DestroyImmediate(_rowNormalATex);
            DestroyImmediate(_rowNormalBTex);
            DestroyImmediate(_dividerTex);
            DestroyImmediate(_selectedEditor);
        }

        private void Refresh()
        {
            _allChips.Clear();
            var guids = AssetDatabase.FindAssets("t:ChipDataSO");
            foreach (var guid in guids)
            {
                var path  = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<ChipDataSO>(path);
                if (asset != null) _allChips.Add(asset);
            }

            _allChips.Sort((a, b) => string.Compare(a.GetType().Name + a.Name,
                                                      b.GetType().Name + b.Name,
                                                      StringComparison.Ordinal));

            _typeNames = _allChips
                .Select(c => c.GetType().Name)
                .Distinct()
                .OrderBy(n => n)
                .Prepend("전체")
                .ToList();

            if (!_typeNames.Contains(_typeFilter))
                _typeFilter = "전체";
        }

        private void OnGUI()
        {
            DrawToolbar();

            EditorGUILayout.BeginHorizontal();
            DrawList();
            DrawDividerV();
            DrawDetail();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("새로고침", EditorStyles.toolbarButton, GUILayout.Width(70)))
                Refresh();

            GUILayout.Space(6);

            GUILayout.Label("검색", EditorStyles.toolbarButton, GUILayout.Width(30));
            _search = EditorGUILayout.TextField(_search, EditorStyles.toolbarSearchField, GUILayout.Width(160));
            if (GUILayout.Button("✕", EditorStyles.toolbarButton, GUILayout.Width(20)))
                _search = "";

            GUILayout.Space(6);

            GUILayout.Label("타입", EditorStyles.toolbarButton, GUILayout.Width(30));
            int curIdx  = _typeNames.IndexOf(_typeFilter);
            int newIdx  = EditorGUILayout.Popup(curIdx < 0 ? 0 : curIdx, _typeNames.ToArray(),
                                                 EditorStyles.toolbarPopup, GUILayout.Width(160));
            _typeFilter = _typeNames[newIdx];

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+ 새 칩", EditorStyles.toolbarButton, GUILayout.Width(70)))
                ShowCreateMenu();

            GUILayout.Label($"총 {_allChips.Count}개", EditorStyles.toolbarButton, GUILayout.Width(60));

            EditorGUILayout.EndHorizontal();
        }

        private void DrawList()
        {
            var filtered = GetFiltered();

            var sideBgStyle = new GUIStyle(GUIStyle.none)
            {
                normal = { background = _sideBgTex }
            };

            EditorGUILayout.BeginVertical(sideBgStyle, GUILayout.Width(ListWidth));

            _listScroll = EditorGUILayout.BeginScrollView(_listScroll, GUILayout.Width(ListWidth));

            string currentGroup = null;

            for (int i = 0; i < filtered.Count; i++)
            {
                var chip      = filtered[i];
                var typeName  = chip.GetType().Name;

                if (typeName != currentGroup)
                {
                    currentGroup = typeName;
                    DrawGroupHeader(typeName);
                }

                DrawChipRow(chip, i);
            }

            if (filtered.Count == 0)
            {
                GUILayout.Space(16);
                GUILayout.Label("칩이 없습니다.", EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawGroupHeader(string typeName)
        {
            GUILayout.Space(4);
            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                normal = { textColor = new Color(0.6f, 0.8f, 1f) },
                padding = new RectOffset(8, 0, 2, 2),
                fontSize = 10
            };
            GUILayout.Label(typeName.Replace("DataSO", "").Replace("ChipData", " Chip"), style);

            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawPreviewTexture(rect, _dividerTex);
        }

        private void DrawChipRow(ChipDataSO chip, int index)
        {
            bool isSelected = _selected == chip;

            var rect = EditorGUILayout.GetControlRect(false, RowHeight, GUILayout.Width(ListWidth));
            bool isHovered = rect.Contains(Event.current.mousePosition);

            Texture2D bg = isSelected ? _rowSelectedTex
                         : isHovered  ? _rowHoverTex
                         : index % 2 == 0 ? _rowNormalATex : _rowNormalBTex;

            if (Event.current.type == EventType.Repaint)
                GUI.DrawTexture(rect, bg);

            float x = rect.x + 4;

            if (chip.Icon != null)
            {
                var preview = AssetPreview.GetAssetPreview(chip.Icon);
                if (preview != null)
                    GUI.DrawTexture(new Rect(x, rect.y + 4, 40, 40), preview, ScaleMode.ScaleToFit);
            }
            else
            {
                var noIconRect = new Rect(x, rect.y + 4, 40, 40);
                EditorGUI.DrawRect(noIconRect, new Color(0.3f, 0.3f, 0.3f));
                GUI.Label(noIconRect, "?", new GUIStyle(EditorStyles.centeredGreyMiniLabel));
            }

            float textX = x + 46;

            GUI.Label(new Rect(textX, rect.y + 6, ListWidth - textX - 4, 18),
                string.IsNullOrEmpty(chip.Name) ? "(이름 없음)" : chip.Name,
                new GUIStyle(EditorStyles.boldLabel));

            GUI.Label(new Rect(textX, rect.y + 24, ListWidth - textX - 4, 14),
                $"ID: {chip.ChipId}   Lv: 1~{chip.MaxLevel}",
                EditorStyles.miniLabel);

            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                SelectChip(chip);
                Repaint();
            }

            if (Event.current.type == EventType.ContextClick && rect.Contains(Event.current.mousePosition))
            {
                ShowRowContextMenu(chip);
                Event.current.Use();
            }
        }

        private void DrawDetail()
        {
            EditorGUILayout.BeginVertical();

            if (_selected == null)
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("← 좌측에서 칩을 선택하세요", EditorStyles.centeredGreyMiniLabel);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                return;
            }

            DrawDetailHeader();

            _detailScroll = EditorGUILayout.BeginScrollView(_detailScroll);

            if (_selectedEditor != null)
                _selectedEditor.OnInspectorGUI();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawDetailHeader()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            GUILayout.Label(
                $"편집 중: {(string.IsNullOrEmpty(_selected.Name) ? _selected.name : _selected.Name)}",
                EditorStyles.boldLabel
            );

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("에셋 선택", EditorStyles.toolbarButton, GUILayout.Width(70)))
                EditorGUIUtility.PingObject(_selected);

            if (GUILayout.Button("Inspector에서 열기", EditorStyles.toolbarButton, GUILayout.Width(120)))
                Selection.activeObject = _selected;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawDividerV()
        {
            var rect = EditorGUILayout.GetControlRect(false, GUILayout.Width(2), GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(rect, DividerColor);
        }

        private void SelectChip(ChipDataSO chip)
        {
            _selected = chip;
            DestroyImmediate(_selectedEditor);
            _selectedEditor = UnityEditor.Editor.CreateEditor(chip);
        }

        private void ShowCreateMenu()
        {
            var menu      = new GenericMenu();
            var chipTypes = GetAllChipDataSOTypes();

            foreach (var type in chipTypes)
            {
                var t = type;
                var displayName = t.Name.Replace("DataSO", "").Replace("ChipData", " Chip");
                menu.AddItem(new GUIContent(displayName), false, () => CreateNewChip(t));
            }

            menu.ShowAsContext();
        }

        private void ShowRowContextMenu(ChipDataSO chip)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("에셋 선택"), false, () => EditorGUIUtility.PingObject(chip));
            menu.AddItem(new GUIContent("Inspector에서 열기"), false, () => Selection.activeObject = chip);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("삭제"), false, () =>
            {
                if (EditorUtility.DisplayDialog("칩 삭제", $"'{chip.Name}' 을(를) 삭제하시겠습니까?", "삭제", "취소"))
                {
                    var path = AssetDatabase.GetAssetPath(chip);
                    if (_selected == chip) { _selected = null; DestroyImmediate(_selectedEditor); }
                    AssetDatabase.DeleteAsset(path);
                    Refresh();
                }
            });
            menu.ShowAsContext();
        }

        private void CreateNewChip(Type type)
        {
            var path = EditorUtility.SaveFilePanelInProject(
                $"새 {type.Name} 생성",
                $"New{type.Name}",
                "asset",
                "저장 경로를 선택하세요"
            );

            if (string.IsNullOrEmpty(path)) return;

            var asset = ScriptableObject.CreateInstance(type) as ChipDataSO;
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Refresh();
            SelectChip(asset);
        }

        private List<ChipDataSO> GetFiltered()
        {
            return _allChips.Where(c =>
            {
                if (_typeFilter != "전체" && c.GetType().Name != _typeFilter) return false;
                if (string.IsNullOrEmpty(_search)) return true;

                var q = _search.ToLower();
                return (c.Name?.ToLower().Contains(q) ?? false)
                    || (c.ChipId?.ToLower().Contains(q) ?? false)
                    || c.GetType().Name.ToLower().Contains(q);
            }).ToList();
        }

        private static List<Type> GetAllChipDataSOTypes()
        {
            var baseType = typeof(ChipDataSO);
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => t != null && !t.IsAbstract && baseType.IsAssignableFrom(t))
                .OrderBy(t => t.Name)
                .ToList();
        }

        private static Texture2D MakeTex(Color c)
        {
            var t = new Texture2D(1, 1);
            t.SetPixel(0, 0, c);
            t.Apply();
            return t;
        }
    }
}
