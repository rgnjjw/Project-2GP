// 베이스 SO에 LevelData 배열 올리기

using System;
using UnityEngine;

namespace _02_Scripts.Chip
{
    [Serializable]
    public class LevelData
    {
        public string Name;
        public string Description;
    }
    public class ChipDataSO : ScriptableObject
    {
        [field: SerializeField] public string ChipId   { get; private set; }
        [field: SerializeField] public string Name     { get; private set; }
        [field: SerializeField] public Sprite Icon     { get; private set; }
        [field: SerializeField] public int MaxLevel { get; private set; } = 5;

        [field: SerializeField] public LevelData[] LevelNameAndDescData { get; private set; }
    }
}