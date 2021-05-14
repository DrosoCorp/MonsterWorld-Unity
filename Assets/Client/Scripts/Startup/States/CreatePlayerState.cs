using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MonsterWorld.Unity.Startup
{
    public class CreatePlayerState : StateMachineBehaviour
    {

        [SerializeField] private AssetReferenceScene _playerCreationSceneReference;
        private AsyncOperationHandle<SceneInstance> _loadplayerCreationSceneHandle;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _loadplayerCreationSceneHandle = _playerCreationSceneReference.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Additive);
            _loadplayerCreationSceneHandle.Completed += (handle) =>
            {
                var scene = _loadplayerCreationSceneHandle.Result.Scene;
            };
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Addressables.UnloadSceneAsync(_loadplayerCreationSceneHandle);
        }
    }
}