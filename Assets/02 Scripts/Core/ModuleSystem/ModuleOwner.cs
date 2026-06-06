using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _02_Scripts.Core.ModuleSystem
{
    public abstract class ModuleOwner : MonoBehaviour //직계 자손만 초기화하게 구조 바꿈
    {
        protected Dictionary<Type, IModule> _moduleDict;

        protected virtual void Awake()
        {
            _moduleDict = GetComponentsInChildren<IModule>()
                .Where(m => BelongsToThisOwner(((MonoBehaviour)m).transform))
                .ToDictionary(module => module.GetType());
            
            InitializeModules();
            AfterInitModules();
        }

        private bool BelongsToThisOwner(Transform t)
        {
            if (t == this.transform) return true;

            t = t.parent;
            while (t != null)
            {
                if (t == this.transform) return true;           
                if (t.GetComponent<ModuleOwner>() != null) return false;
                t = t.parent;
            }
            return false;
        }   

        protected virtual void InitializeModules()
        {
            foreach(IModule module in _moduleDict.Values)
                module.Initialize(this);
        }

        protected virtual void AfterInitModules()
        {
            foreach(IAfterInitModule module in _moduleDict.Values.OfType<IAfterInitModule>())
                module.AfterInit();
        }

        public T GetModule<T>()
        {
            if (_moduleDict.TryGetValue(typeof(T), out IModule module))
                return (T)module;

            IModule findModule = _moduleDict.Values.FirstOrDefault(m => m is T);

            if (findModule is T castModule)
                return castModule;
            
            return default(T);
        }
        
        public IEnumerable<T> GetModules<T>()
        {
            return _moduleDict.Values.OfType<T>();
        }
    }
}