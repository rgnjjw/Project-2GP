using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace _02_Scripts.Core.FSMSystem.Editor
{
    [CustomEditor(typeof(StateListSO))]
    public class StateListSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset editorView = default;

        private Button _folderBtn;
        private Button _generateBtn;
        private Label _folderPathLabel;

        private string _folderPath;
        private StateListSO _targetData;
        
        public override VisualElement CreateInspectorGUI()
        {
            _targetData = target as StateListSO;
            
            VisualElement root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            editorView.CloneTree(root);

            _folderBtn = root.Q<Button>("FolderBtn");
            _generateBtn = root.Q<Button>("GenerateBtn");
            _folderPathLabel = root.Q<Label>("SelectedFolderLabel");
            _folderPathLabel.text = "No folder selected";

            _folderBtn.clicked += HandleFolderBtnClick;
            _generateBtn.clicked += HandleGenerateBtnClick;

            if (_targetData != null && !string.IsNullOrEmpty(_targetData.generatePath))
            {
                _folderPath = _targetData.generatePath;
                _folderPathLabel.text = FileUtil.GetProjectRelativePath(_targetData.generatePath);
            }
            
            return root;
        }

        private void HandleGenerateBtnClick()
        {
            if (string.IsNullOrEmpty(_folderPath) || !Directory.Exists(_folderPath))
            {
                EditorUtility.DisplayDialog("Error", "폴더를 선택하지 않았거나 올바르지 않습니다.", "OK");
                return;
            }

            int index = 0;
            string enumString = string.Join(",", _targetData.states.Select(so =>
            {
                so.assetIndex = index;
                EditorUtility.SetDirty(so);
                return $"{so.stateName} = {index++}";
            }));

            string ns = FileUtil.GetProjectRelativePath(_folderPath).Substring("Assets/".Length);
            ns = string.Join(".", ns.Split('/').Select(part =>
                char.IsDigit(part[0]) ? "_" + part : part));

            ns = ns.Replace(" ", "_");

            string code = string.Format(CodeFormat.EnumFormat, ns, _targetData.enumName, enumString);
            
            File.WriteAllText($"{_folderPath}/{_targetData.enumName}.cs", code);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void HandleFolderBtnClick()
        {
            _folderPath = EditorUtility.OpenFolderPanel("폴더를 선택하세요", _folderPath, "");

            if (!string.IsNullOrEmpty(_folderPath))
            {
                Debug.Log(_folderPath);
                _targetData.generatePath = _folderPath;
                _folderPathLabel.text = FileUtil.GetProjectRelativePath(_folderPath);
                
                EditorUtility.SetDirty(_targetData);
                AssetDatabase.SaveAssets();
            }
        }
    }
}