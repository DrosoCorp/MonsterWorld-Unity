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

        public string GameModeName => _name;
        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        private void Start()
        {
            if (_loadOnStart)
            {
                Load();
            }
        }

        public void Load()
        {
            if (!IsLoaded && !IsLoading)
            {
                IsLoading = true;
                _sceneLoader = new SceneLoader();
                StartCoroutine(InitializeSystemsCoroutine());
            }
        }

        private IEnumerator InitializeSystemsCoroutine()
        {
            var systems = GetComponentsInChildren<GameSystem>();
            foreach (var system in systems)
            {
                system.BindSceneLoader(_sceneLoader);
                system.Initialize();
                while (!system.IsReady)
                {
                    yield return null;
                }
            }

            IsLoaded = true;
            IsLoading = false;
            if (GameModeLoadComplete != null)
            {
                GameModeLoadComplete.Invoke();
            }
        }
    }
}