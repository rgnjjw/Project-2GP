using System;
using System.Collections.Generic;

namespace _02_Scripts.Core.Utility
{
    public static class EventBus
    {
        private static Dictionary<Type, Delegate> _handlers = new();

        public static void Subscribe<T>(Action<T> handler)
        {
            if(handler == null) return;
            if(_handlers.TryGetValue(typeof(T),out Delegate existing))
                _handlers[typeof(T)] = (Action<T>)existing + handler;//원본 액션에 추가해주기
            else
                _handlers[typeof(T)] = handler;//새로 만들어서 딕셔너리에 넣기
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (!_handlers.TryGetValue(typeof(T), out Delegate existing)) return;
            //만약 딕셔너리에 구독해제할 액션이 없다면 리턴 (없는데 구독해제 할 수는 없으니까)
            var newHandler = (Action<T>)existing - handler;
            //새로 만드는 이유는 구독 해제했을떄 널이 뜰수 있는데 이걸 체크를 안하면 딕셔너리에 널값이 들어갈수 있어서
            
            if(newHandler == null)
                _handlers.Remove(typeof(T));
            else
                _handlers[typeof(T)] = newHandler;
        }

        public static void Publish<T>(T message)
        {
            if (_handlers.TryGetValue(typeof(T), out Delegate existing))
            {
                if(existing is Action<T> action)
                    action?.Invoke(message);
            }
        }
        
        public static void Clear() //씬 전환시나 게임 리셋할때 사용
        {
            _handlers.Clear();
        }
    }
}