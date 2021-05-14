using MonsterWorld.Unity.Network;
using MonsterWorld.Unity.Network.Client;
using System;
using UnityEngine;

namespace MonsterWorld.Unity.Startup
{
    public class ConnectToServerState : StateMachineBehaviour
    {
        // We will never have duplicate animators
        // It's safe to store a reference
        private Animator _Animator;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _Animator = animator;
            ClientNetworkManager.OnConnectionResponsePacket += OnResponse;

            if (ClientNetworkManager.Connected)
            {
                var connectionPacket = new ConnectionPacket()
                {
                    token = StartupFSMContext.connectionToken
                };

                ClientNetworkManager.SendPacket(ref connectionPacket);
                Debug.Log("[ConnectToServerState] Connecting to server");
            }
            else
            {
                Debug.Log("[ConnectToServerState] Could not connect to the server...");
                _Animator.SetTrigger(StartupFSMContext.Parameters.ConnectionToServerFailedId);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ClientNetworkManager.OnConnectionResponsePacket -= OnResponse;
        }

        private void OnResponse(ref ConnectionResponsePacket packet)
        {
            Debug.Log("Response :" + packet.responseType);
            switch (packet.responseType)
            {
                case ConnectionResponsePacket.ResponseType.Success:
                    Debug.Log("[ConnectToServerState] Connection Success !");
                    _Animator.SetBool(StartupFSMContext.Parameters.PlayerConnectedId, true);
                    break;
                case ConnectionResponsePacket.ResponseType.RequestPlayerCreation:
                    Debug.Log("[ConnectToServerState] Requesting Player Creation.");
                    _Animator.SetTrigger(StartupFSMContext.Parameters.RequestPlayerDataId);
                    break;
                case ConnectionResponsePacket.ResponseType.InvalidToken:
                    Debug.Log("[ConnectToServerState] Invalid Token");
                    _Animator.SetTrigger(StartupFSMContext.Parameters.ConnectionToServerFailedId);
                    break;
            }
        }
    }
}
