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

            var types = AppDomain.CurrentDomain.GetAssemblies()//게임의 모든 어셈블리 가져오기
                .SelectMany(a => a.GetTypes())//SelectMany : 각 어셈블리에서 클래스들을 꺼내서 하나의 목록으로 합침
                .Where(t => typeof(IChip).IsAssignableFrom(t)//Where : 거르기,IsAssignableFrom : IChip 구현 했는지
                         && !t.IsInterface //인터페이스가 아니여야함
                         && !t.IsAbstract); //추상이 아니여야함

            foreach (var type in types)
            {
                var attr = type.GetCustomAttribute<ChipAttribute>();//GetCustomAttribute 모름
                if (attr != null)
                    _registry[attr.ChipId] = type; //딕셔너리에 넣기
            }
        }

        public static IChip Create(string chipId)
        {
            if (_registry.TryGetValue(chipId, out var type))
                return (IChip)Activator.CreateInstance(type);//객체 생성
            
            return null;
        }
    }
}