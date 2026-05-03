using System;

namespace _02_Scripts.Chip
{
    [AttributeUsage(AttributeTargets.Class)]//클래스만 사용가능
    public class ChipAttribute  : Attribute
    {
        public string ChipId { get;}
        public ChipAttribute(string chipId) => ChipId = chipId;//매개변수로 ID를 받아서 넣음
    }
}