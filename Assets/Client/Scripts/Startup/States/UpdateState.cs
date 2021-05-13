using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MonsterWorld.Unity.Startup
{
    public class UpdateState : StateMachineBehaviour
    {

        [SerializeField] private AssetReferenceScene _updaterSceneReference;
        private AsyncOperationHandle<SceneInstance> _loadUpdaterSceneHandle;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _loadUpdaterSceneHandle = _updaterSceneReference.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Additive);
            _loadUpdaterSceneHandle.Completed += (handle) =>
            {
                var scene = _loadUpdaterSceneHandle.Result.Scene;
                var updater = scene.GetRootGameObjects()[0].GetComponent<Updater>();
                updater.UpdateTerminated += () => animator.SetBool(StartupFSMContext.Parameters.UpdateCompleteId, true);
            };
        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Addressables.UnloadSceneAsync(_loadUpdaterSceneHandle);
        }
    }
}
