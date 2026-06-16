using UnityEngine;

namespace _02_Scripts.UI.StyleBar
{
    [CreateAssetMenu(fileName = "StyleScoreDataSO", menuName = "Style/StyleScoreDataSO", order = 0)]
    public class StyleScoreDataSO : ScriptableObject
    {
        [field: SerializeField] public float KillScore { get;private set; }
        [field:SerializeField] public float HeadshotScore { get;private set; }
        [field:SerializeField] public float DashKillScore { get;private set; }
        [field:SerializeField] public float HighJumpScore { get;private set; }
        [field:SerializeField] public float FastMoveScore { get;private set; }
    }
}