using System;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class GunAnimationEvent : MonoBehaviour
    {
        public event Action OnSkillFire;

        public void SkillFire() => OnSkillFire?.Invoke();
    }
}
