//-----------------------------------------------------------------
// File:         SceneLoader.cs
// Description:  Load and Unload Scenes
// Module:       Unity Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;

using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace MonsterWorld.Unity
{
    public enum SceneLoadingResult
    {
        Succeeded,
        Failed
    }

    public class SceneLoader
    {
        public class SceneLoadingOperation
        {
            public Action<SceneLoadingResult> Completed;
        }

        private Queue<AssetReferenceScene> _scenesToLoad;
        private bool _isReady;
        private SceneLoadingOperation _operation;

        public bool IsReady => _isReady;

        public SceneLoader()
        {
            _isReady = true;
            _operation = new SceneLoadingOperation();
        }

        public SceneLoadingOperation LoadScenes(params AssetReferenceScene[] scenes)
        {
            if (scenes.Length == 0) return null;

            _scenesToLoad = new Queue<AssetReferenceScene>(scenes);
            _isReady = false;

            _scenesToLoad.Dequeue().LoadSceneAsync(LoadSceneMode.Additive).Completed += ContinueSceneLoading;
            return _operation;
        }

        private void ContinueSceneLoading(AsyncOperationHandle<SceneInstance> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (_scenesToLoad.Count > 0)
                {
                    _scenesToLoad.Dequeue().LoadSceneAsync(LoadSceneMode.Additive).Completed += ContinueSceneLoading;
                }
                else
                {
                    _isReady = true;
                    if (_operation.Completed != null)
                    {
                        _operation.Completed.Invoke(SceneLoadingResult.Succeeded);
                        _operation.Completed = null;
                    }
                }
            }
            else
            {
                if (_operation.Completed != null)
                {
                    _operation.Completed.Invoke(SceneLoadingResult.Failed);
                    _operation.Completed = null;
                }
            }
        }
    }
}