using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _02_Scripts.Core.ModuleSystem
{
    public abstract class ModuleOwner : MonoBehaviour
    {
        protected List<IModule> _modules;

        protected virtual void Awake()
        {
            _modules = GetComponentsInChildren<IModule>().ToList();
            InitializeModules();
            AfterInitModules();
        }

        protected virtual void InitializeModules()
        {
            foreach (IModule module in _modules)
                module.Initialize(this);
        }

        protected virtual void AfterInitModules()
        {
            foreach (IAfterInitModule module in _modules.OfType<IAfterInitModule>())
                module.AfterInit();
        }

        public T GetModule<T>()
        {
            return _modules.OfType<T>().FirstOrDefault();
        }

        public IEnumerable<T> GetModules<T>()
        {
            return _modules.OfType<T>();
        }
    }
}