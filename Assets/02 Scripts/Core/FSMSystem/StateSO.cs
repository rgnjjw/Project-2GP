using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Core.FSMSystem
{
    [CreateAssetMenu(fileName = "State data", menuName = "Agent/State data", order = 20)]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string className;
        public int assetIndex;
        public AnimParamSO stateParam;
    }
}