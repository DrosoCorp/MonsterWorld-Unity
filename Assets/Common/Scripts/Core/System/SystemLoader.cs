//-----------------------------------------------------------------
// File:         SystemContainer.cs
// Description:  Holds system and their states
// Module:       Core
// Author:       Noé Masse
// Date:         21/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MonsterWorld.Core
{
    public class SystemLoader
    {
        private Dictionary<Type, GameSystem> _systems;
        private List<AsyncOperationHandle> _loadingHandles;

        public Action Completed;

        public SystemLoader(Dictionary<Type, GameSystem> systems)
        {
            _systems = systems;
        }

        public void LoadSystems()
        {
            var loadingOrder = new List<Type>(_systems.Count);
            foreach (var kvp in _systems)
            {
                AddSystemToLoadAndBindDependencies(loadingOrder, kvp.Key);
            }

            _loadingHandles = new List<AsyncOperationHandle>(_systems.Count);
            for (int i = 0; i < loadingOrder.Count; i++)
            {
                var op = new LoadSystemOperation();
                op.Init(_systems[loadingOrder[i]]);
                _loadingHandles.Add(Addressables.ResourceManager.StartOperation(op, i == 0 ? default : _loadingHandles[i - 1]));
            }

            _loadingHandles[_loadingHandles.Count - 1].Completed += LoadingComplete;
        }

        private void AddSystemToLoadAndBindDependencies(List<Type> loadingOrder, Type type)
        {
            if (loadingOrder.Contains(type)) return;

            var dependencyFields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(IsDependencyFieldValid);

            foreach (var depencyField in dependencyFields)
            {
                var dependencyType = depencyField.FieldType;

                if (_systems.TryGetValue(dependencyType, out var dependencySystem))
                {
                    if (!loadingOrder.Contains(dependencyType))
                    {
                        AddSystemToLoadAndBindDependencies(loadingOrder, dependencyType);
                    }
                    depencyField.SetValue(_systems[type], dependencySystem);
                }
                else
                {
                    Debug.LogError("[SystemLoader] " + dependencyType.Name + " is needed by " + type.Name + " but is not registered.");
                }
            }

            loadingOrder.Add(type);
        }

        private void LoadingComplete(AsyncOperationHandle handle)
        {
            for (int i = 0; i < _loadingHandles.Count; i++)
            {
                Addressables.Release(_loadingHandles[i]);
            }
            _loadingHandles.Clear();
            Completed.Invoke();
        }

        public List<GameSystem> GetSystems()
        {
            return new List<GameSystem>(_systems.Values.Where(system => system.IsReady));
        }

        private bool IsDependencyFieldValid(FieldInfo field)
        {
            return field.FieldType.IsSubclassOf(typeof(GameSystem)) && field.GetCustomAttributes(typeof(SystemDependencyAttribute), true).Length > 0;
        }
    }
}