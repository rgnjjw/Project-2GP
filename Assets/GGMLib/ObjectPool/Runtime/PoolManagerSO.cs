using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GGMLib.ObjectPool.Runtime
{
	[CreateAssetMenu(fileName = "PoolManager", menuName = "Lib/Pool/PoolManager", order = 10)]
	public class PoolManagerSO : ScriptableObject
	{
		// 런타임 어디서든 풀에 접근할 수 있도록 InitializePool에서 설정되는 전역 접근자.
		// (SoundManager 등 프로젝트의 다른 싱글톤과 동일한 사용 패턴)
		public static PoolManagerSO Instance { get; private set; }

		public List<PoolItemSO> itemList = new();
		private Dictionary<PoolItemSO, Pool> _pools;
		private Transform _rootTrm;

		public void InitializePool(Transform rootTrm)
		{
			Instance = this;
			_rootTrm = rootTrm;
			_pools = new  Dictionary<PoolItemSO, Pool>();

			foreach (PoolItemSO item in itemList)
			{
				IPoolable poolable = item.prefab.GetComponent<IPoolable>();
				Debug.Assert(poolable != null, $"PoolItem은 반드시 IPoolable을 가져야 합니다. {item.prefab.name}");

				Pool pool = new Pool(poolable, _rootTrm, item.initCount); 
				_pools.Add(item, pool); // 풀 딕셔너리에 item을 기준으로 만들어 넣어준다.
			}
		}
		// 제네릭을 통해 원하는 아이템을 가져오게 한다.
		public T Pop<T>(PoolItemSO item) where T : IPoolable
		{
			Debug.Assert(_rootTrm != null, "풀 매니저는 초기화 후 사용해야 합니다.");

			if (_pools.TryGetValue(item, out Pool pool))
			{
				return (T)pool.Pop();
			}

			return default;
		}

		public void Push(IPoolable item)
		{
			Debug.Assert(_rootTrm != null, $"풀 매니저는 초기화 후 사용핻야 합니다.");
			if (_pools.TryGetValue(item.Item, out Pool pool))
			{
				pool.Push(item);	
			}
		}
	}
}