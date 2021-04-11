//-----------------------------------------------------------------
// File:         ClientAuth.cs
// Description:  Class that initialize the network and offer methods to connect
// Module:       Network.Client
// Author:       Thomas Hervé
// Date:         30/03/2021
//-----------------------------------------------------------------
using UnityEngine;
using AWS;
using System;
using System.Threading.Tasks;

namespace MonsterWorld.Unity.Network.Client
{
    public class ClientAuth : MonoBehaviour
    {
        public static bool ConnectedToServer {
            get 
            {
                return _connectedToServer;
            }
        }
        private static bool _connectedToServer = false;

        private Action onSuccess = null;
        private bool connected = false;

        ICognito cognito
        {
            get
            {
                if (_cognito == null)
                    _cognito = new Cognito();
                return _cognito;
            }
        }
        ICognito _cognito = null;

        // Start is called before the first frame update
        void Start()
        {            
            //Init connection with the server
            ClientNetworkManager.Init();
            ClientNetworkManager.RegisterHandler<ConnectPacket>();
            ClientNetworkManager.RegisterHandler<ValidateConnectionPacket>((paquet) => {
                if(paquet.tokenValid)
                {
                    _connectedToServer = true;
                    if(onSuccess != null)
                    {
                        onSuccess();
                    }
                }
            });
            ClientNetworkManager.Connect(Connect);
        }

        // Update is called once per frame
        void Update()
        {
            ClientNetworkManager.UpdateClient();
        }

        public void SignIn(string email, string password, Action<Exception> onFailure = null, Action onSuccess = null)
        {
            if (connected)
            {
                this.onSuccess = onSuccess;
                cognito.TrySignInRequest(email, password,
                    onFailure,
                    (token) =>
                    {
                        ClientNetworkManager.SendPacket(
                                new ConnectPacket()
                                {
                                    token = token
                                }
                            );
                    }
                );
            } else
            {
                onFailure(new Exception("Not connected to the server, please wait. if this error is persistent," +
                    " your internet connection may be down or the server is not available."));
            }
        }

        public void SignUp(string email, string password, Action<Exception> onFailure = null, Action onSuccess = null)
        {
            cognito.TrySignUpRequest(email, password,
                onFailure,
                onSuccess
            );
        }

        private void Connect()
        {
            connected = true;
        }
    }
}
