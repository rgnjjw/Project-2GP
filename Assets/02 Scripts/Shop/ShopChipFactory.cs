using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace _02_Scripts.Shop
{
    public static class ShopChipFactory
    {
        private static Dictionary<string, Type> _registry;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BuildRegistry()
        {
            _registry = new Dictionary<string, Type>();

            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in allAssemblies)
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

                foreach (var type in types)
                {
                    if (type == null || type.IsInterface || type.IsAbstract) continue;
                    if (!typeof(IShopPassiveEffect).IsAssignableFrom(type)) continue;

                    var attr = type.GetCustomAttribute<ShopChipAttribute>();
                    if (attr != null)
                        _registry[attr.ChipId] = type;
                    else
                        Debug.LogWarning($"[ShopChipFactory] IShopPassiveEffect 구현체지만 [ShopChip] 어트리뷰트 없음: {type.FullName}");
                }
            }

            Debug.Log($"[ShopChipFactory] 등록된 패시브 칩: {string.Join(", ", _registry.Keys)}");
        }

        public static IShopPassiveEffect Create(string chipId)
        {
            if (_registry == null)
            {
                Debug.LogError("[ShopChipFactory] 레지스트리가 null");
                return null;
            }
            if (_registry.TryGetValue(chipId, out var type))
                return (IShopPassiveEffect)Activator.CreateInstance(type);

            return null;
        }
    }
}
