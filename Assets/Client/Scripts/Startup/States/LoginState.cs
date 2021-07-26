using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MonsterWorld.Unity.Startup
{
    public class LoginState : StateMachineBehaviour
    {

        [SerializeField] private AssetReferenceScene _loginSceneReference;
        private AsyncOperationHandle<SceneInstance> _loadLoginSceneHandle;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _loadLoginSceneHandle = _loginSceneReference.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Additive);
            _loadLoginSceneHandle.Completed += (handle) =>
            {
                var scene = _loadLoginSceneHandle.Result.Scene;
                var clientLogin = scene.GetRootGameObjects()[0].GetComponent<ClientLogin>();
                clientLogin.OnConnectionToken += (token) =>
                {
                    StartupFSMContext.connectionToken = token;
                    animator.SetBool(StartupFSMContext.Parameters.HasConnectionTokenId, true);
                    animator.SetTrigger(StartupFSMContext.Parameters.LoginId);
                    Addressables.UnloadSceneAsync(_loadLoginSceneHandle);
                };
            };
        }
    }
}