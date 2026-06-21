using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGMLib.EventChannelSystem
{
	[CreateAssetMenu(fileName = "Event Channel", menuName = "Lib/EventChannel", order = 0)]
	public class GameEventChannelSO : ScriptableObject
	{
		private Dictionary<Type, Action<GameEvent>> _events = new();
		private Dictionary<Delegate, Action<GameEvent>> _lookUp = new();

		// 인스턴스와 바인딩된 함수를 보는게 Delegate <= 2번 구독이 안되게 하는거
		public void AddListener<T>(Action<T> handler) where T : GameEvent
		{
			if (_lookUp.ContainsKey(handler)) return;

			Action<GameEvent> castHandler = (evt) => handler(evt as T);
			_lookUp[handler] = castHandler;
			Type eventType = typeof(T);
			if (_events.ContainsKey(eventType))
			{
				_events[eventType] += castHandler;
			}
			else
			{
				_events[eventType] = castHandler;
			}
		}
		
		public void RemoveListener<T>(Action<T> handler) where T : GameEvent
		{
			Type eventType = typeof(T);
			if (_lookUp.TryGetValue(handler, out Action<GameEvent> action))
			{
				if (_events.TryGetValue(eventType, out Action<GameEvent> internalAction))
				{
					internalAction -= action;
					if (internalAction == null)
					{
						_events.Remove(eventType);
					}
					else // 남아있다면 다시 넣어줌
					{
						_events[eventType] = internalAction;
					}
				}

				_lookUp.Remove(handler); // 룩업 테이블에서는 제거
			}
		}

		public void RaiseEvent(GameEvent evt)
		{
			if (_events.TryGetValue(evt.GetType(), out Action<GameEvent> action))
			{
				action?.Invoke(evt);
			}
		}

		public void Clear()
		{
			_events.Clear();
			_lookUp.Clear();
		}
	}
}