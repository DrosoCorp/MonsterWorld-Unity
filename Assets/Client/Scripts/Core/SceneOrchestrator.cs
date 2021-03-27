//-----------------------------------------------------------------
// File:         SceneLoader.cs
// Description:  Load and Unload Scenes
// Module:       Unity Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System.Collections.Generic;

using MonsterWorld.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MonsterWorld.Unity
{
    public class SceneOrchestrator
    {
        private List<Scene> _loadedScenes = null;
        private List<AsyncOperation> _loadingOperations = null;

        public SceneOrchestrator(int storageCapacity, int loadCapacity)
        {
            _loadedScenes = new List<Scene>(storageCapacity);
            _loadingOperations = new List<AsyncOperation>(loadCapacity);
        }

        public void LoadScenes(params string[] scenes)
        {
            foreach (var scene in scenes)
            {
                var sceneLoadOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                if (sceneLoadOperation != null)
                {
                    _loadingOperations.Add(sceneLoadOperation);
                }
                else
                {
                    Debug.LogWarning("[SceneOrchestrator] Unable to load scene: " + scene);
                }
            }
        }

        public void Update()
        {

        }
    }

}