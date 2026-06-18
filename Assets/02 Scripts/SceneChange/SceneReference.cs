using System;
using UnityEngine;

namespace _02_Scripts.SceneChange
{
    [Serializable]
    public class SceneReference
    {
        [SerializeField] private string sceneName;
        public string SceneName => sceneName;
    }
}