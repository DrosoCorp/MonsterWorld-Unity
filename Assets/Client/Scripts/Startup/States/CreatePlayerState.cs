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
        private AsyncOperationHandle<SceneInstance> _loadPlayerCreationSceneHandle;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _loadPlayerCreationSceneHandle = _playerCreationSceneReference.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Additive);
            _loadPlayerCreationSceneHandle.Completed += (handle) =>
            {
                var scene = _loadPlayerCreationSceneHandle.Result.Scene;
                var playerCreation = scene.GetRootGameObjects()[0].GetComponent<PlayerCreation>();
                playerCreation.OnCreationSuccess += () =>
                {
                    animator.SetBool(StartupFSMContext.Parameters.PlayerConnectedId, true);
                };
            };
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Addressables.UnloadSceneAsync(_loadPlayerCreationSceneHandle);
        }
    }
}