using System;

namespace _02_Scripts.Shop
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShopChipAttribute : Attribute
    {
        public string ChipId { get; }
        public ShopChipAttribute(string chipId) => ChipId = chipId;
    }
}
