using MonsterWorld.Unity.Network.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MonsterWorld.Unity.Startup
{
    public class TryAutoAuthenticationState : StateMachineBehaviour
    {
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ClientNetworkService.Instance.Authenticator.TrySignInRequestRefreshToken(
                PlayerPrefs.GetString("RefreshToken"),
                // Failure
                (_) =>
                {
                    animator.SetBool(StartupFSMContext.Parameters.HasConnectionTokenId, false);
                    animator.SetTrigger(StartupFSMContext.Parameters.AutoAuthenticationFailureId);
                    Debug.Log("AutoAuthentication Failure. Loading Login Menu...");
                },
                // Success
                (token) =>
                {
                    animator.SetBool(StartupFSMContext.Parameters.HasConnectionTokenId, true);
                    animator.SetTrigger(StartupFSMContext.Parameters.AutoAuthenticationSuccessId);
                    StartupFSMContext.connectionToken = token;
                    Debug.Log($"AutoAuthentication Success ! (Token : {token})");
                }
            );
        }
    }
}
