//-----------------------------------------------------------------
// File:         GameMode.cs
// Description:  Define a game mode, contains all systems needed
// Module:       Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;

using UnityEngine;

namespace MonsterWorld.Core
{
    public class GameMode : IDisposable
    {
        private Dictionary<Type, GameSystem> _systems;
        private SystemLoader _systemLoader;
        private bool _isReady;

        public GameMode()
        {
            _systems = new Dictionary<Type, GameSystem>();
            _systemLoader = new SystemLoader(_systems);
            _isReady = false;
        }

        public void AddSystem<T>() where T : GameSystem, new()
        {
            var type = typeof(T);
            if (_systems.ContainsKey(type))
            {
                Debug.LogWarning("[SystemContainer] " + type.Name + " is already registered.");
            }
            else
            {
                _systems.Add(type, new T());
            }
        }

        public void Load(Action loadCallback)
        {
            _systemLoader.Completed += () =>
            {
                _isReady = true;
                loadCallback.Invoke();
            };
            _systemLoader.LoadSystems();
        }

        public void Update()
        {
            if (!_isReady) return;

            foreach (var sys in _systems)
            {
                sys.Value.Update();
            }
        }

        public void Dispose()
        {
            foreach (var sys in _systems)
            {
                sys.Value.Dispose();
            }
        }
    }
}