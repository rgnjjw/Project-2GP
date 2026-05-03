using _02_Scripts.Core.AnimationSystem;
using UnityEngine;

namespace _02_Scripts.Core.FSMSystem
{
    [CreateAssetMenu(fileName = "State DataSO", menuName = "Agent/State DataSO", order = 20)]
    public class StateSO : ScriptableObject
    {
        public string stateName;
        public string className;
        public int assetIndex;
        public AnimParamSO stateParam;
    }
}