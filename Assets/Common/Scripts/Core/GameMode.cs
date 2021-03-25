//-----------------------------------------------------------------
// File:         GameMode.cs
// Description:  Define a GameMode
// Module:       Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace MonsterWorld.Unity
{
    [DisallowMultipleComponent]
    public class GameMode : MonoBehaviour
    {
        [SerializeField] private string _name = "GameMode";
        [SerializeField] private bool _loadOnStart = true;
        public UnityEvent GameModeLoadComplete = null;

        private SceneLoader _sceneLoader = null;
        private bool _isLoaded = false;

        public string GameModeName => _name;
        public bool IsLoaded => _isLoaded;

        private void Start()
        {
            _sceneLoader = new SceneLoader();
            if (_loadOnStart)
            {
                Load();
            }
        }

        public void Load()
        {
            if (!_isLoaded)
            {
                StartCoroutine(InitializeSystemsCoroutine());
            }
        }

        private IEnumerator InitializeSystemsCoroutine()
        {
            var systems = GetComponents<GameSystem>();
            foreach (var system in systems)
            {
                system.BindSceneLoader(_sceneLoader);
                system.Initialize();
                while (!system.IsReady)
                {
                    yield return null;
                }
            }

            _isLoaded = true;
            if (GameModeLoadComplete != null)
            {
                GameModeLoadComplete.Invoke();
            }
        }
    }
}