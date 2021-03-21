//-----------------------------------------------------------------
// File:         ViewSystem.cs
// Description:  A Client-side Game System
// Module:       Unity Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System.Collections.Generic;

using MonsterWorld.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace MonsterWorld.Unity
{
    public abstract class UnitySystem : GameSystem
    {
        private List<Scene> _scenes;
        private List<AsyncOperationHandle<SceneInstance>> _loadedSceneHandles;
        private Queue<string> _scenesToLoad;

        public virtual void FindGameObjects() { }

        public void LoadScenes(params string[] sceneNames)
        {
            _scenes = new List<Scene>(sceneNames.Length);
            _loadedSceneHandles = new List<AsyncOperationHandle<SceneInstance>>(sceneNames.Length);
            _scenesToLoad = new Queue<string>(sceneNames);

            Addressables.LoadSceneAsync(_scenesToLoad.Dequeue(), LoadSceneMode.Additive).Completed += ContinueSceneLoading;
        }

        private void ContinueSceneLoading(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _scenes.Add(handle.Result.Scene);
                if (_scenesToLoad.Count > 0)
                {
                    Addressables.LoadSceneAsync(_scenesToLoad.Dequeue(), LoadSceneMode.Additive).Completed += ContinueSceneLoading;
                }
                else
                {
                    FindGameObjects();
                    TerminateInitialization();
                }

            }
        }

        public void UnloadScenes()
        {
            foreach (var sceneHandle in _loadedSceneHandles)
            {
                Addressables.UnloadSceneAsync(sceneHandle);
            }
            _scenes.Clear();
            _loadedSceneHandles.Clear();
        }

        public GameObject Find(string name)
        {
            return GameObject.Find(name);
        }
    }
}
