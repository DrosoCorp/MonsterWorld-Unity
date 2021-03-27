//-----------------------------------------------------------------
// File:         GameSystem.cs
// Description:  Base class for all game systems.
// Module:       Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using UnityEngine;

namespace MonsterWorld.Unity
{
    public abstract class GameSystem : MonoBehaviour
    {
        private bool _isReady = false;
        private SceneLoader _sceneLoader = null;

        public bool IsReady => _isReady;

        public abstract void Initialize();

        public abstract void Dispose();

        public void BindSceneLoader(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        protected void TerminateInitialization()
        {
            _isReady = true;
        }

        protected T GetSystem<T>() where T : GameSystem
        {
            return GetComponent<T>();
        }

        protected SceneLoader.SceneLoadingOperation LoadScenes(params AssetReferenceScene[] scenes)
        {
            if (_sceneLoader != null)
            {
                return _sceneLoader.LoadScenes(scenes);
            }
            else
            {
                Debug.LogError("[GameSystem] Scene Loader not found.");
                return null;
            }
        }

        protected void UnloadScenes(params AssetReferenceScene[] scenes)
        {
            foreach (var scene in scenes)
            {
                scene.UnLoadScene();
            }
        }
    }
}
