using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using _02_Scripts.Agent;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _02_Scripts.Core.FSMSystem.Editor
{
    [CustomEditor(typeof(StateSO))]
    public class StateSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset editorView = default;

        private StateSO _targetData;
        
        public override VisualElement CreateInspectorGUI()
        {
            _targetData = target as StateSO;
            
            VisualElement root = new VisualElement();

            editorView.CloneTree(root);

            FillDropdownField(root);
            
            return root;
        }

        private void FillDropdownField(VisualElement root)
        {
            DropdownField field = root.Q<DropdownField>("ClassNameDropdown");
            
            //StateSO 클래스가 속해있는 어셈블리를 가져온다.(모든 어쎔을 가져올 수도 있지만, 그럼 너무 느리다)
            Assembly stateAssembly = Assembly.GetAssembly(typeof(StateSO));
            
            //열거 가능한 문자열 목록
            IEnumerable<string> choices = stateAssembly.GetTypes()//타입을 배열로 받는다
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(AgentState)))//이게 AgentState의 자식인지 확인한다
                .Select(type => type.FullName);//찾을거의 이름을 넣음
            
            field.choices.AddRange(choices);
            
            if (_targetData != null && field.choices.Count > 0 && string.IsNullOrEmpty(_targetData.className))
            {
                _targetData.className = field.choices.First();
                EditorUtility.SetDirty(_targetData);
            }
            
            AssetDatabase.SaveAssetIfDirty(_targetData);
        }
    }
}