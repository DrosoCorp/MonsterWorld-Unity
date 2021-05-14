using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MonsterWorld.Unity.Startup
{
    public class LoadGameModeState : StateMachineBehaviour
    {
        [SerializeField] private AssetReferenceGameObject _GameMode;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _GameMode.InstantiateAsync().Completed += (handle) => handle.Result.GetComponent<GameMode>().Load();
            Destroy(animator.gameObject);
        }
    }
}