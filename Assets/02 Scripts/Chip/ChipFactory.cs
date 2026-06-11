using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace _02_Scripts.Chip
{
    public static class ChipFactory
    {
        private static Dictionary<string, Type> _registry;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]//씬 로드전에 이 함수를 자동으로 실행
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
                    if (!typeof(IChip).IsAssignableFrom(type)) continue;

                    var attr = type.GetCustomAttribute<ChipAttribute>();
                    if (attr != null)
                        _registry[attr.ChipId] = type;
                    else
                        Debug.LogWarning($"[ChipFactory] IChip 구현체지만 [Chip] 어트리뷰트 없음: {type.FullName}");
                }
            }

            Debug.Log($"[ChipFactory] 등록된 칩: {string.Join(", ", _registry.Keys)}");
        }

        public static IChip Create(string chipId)
        {
            if (_registry == null)
            {
                Debug.LogError("[ChipFactory] 레지스트리가 null - BuildRegistry가 실행되지 않음");
                return null;
            }
            if (_registry.TryGetValue(chipId, out var type))
                return (IChip)Activator.CreateInstance(type);//객체 생성

            Debug.LogError($"[ChipFactory] '{chipId}' 없음. 등록된 칩: {string.Join(", ", _registry.Keys)}");
            return null;
        }
    }
}
