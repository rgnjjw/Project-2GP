using UnityEngine;

namespace _02_Scripts.Core.AnimationSystem
{
    [CreateAssetMenu(fileName = "Anim param data", menuName = "Lib/AnimParam", order = 20)]
    public class AnimParamSO : ScriptableObject
    {
        [field: SerializeField] public string ParamName { get; private set; }
        [field: SerializeField] public int ParamHash { get; private set; }

        private void OnValidate()
        {
            ParamHash = Animator.StringToHash(ParamName);
        }
    }
}