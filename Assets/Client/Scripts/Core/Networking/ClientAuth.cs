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
        private Action<byte> onFailure = null;

        // Username
        Action<byte> FailureChooseName = null;
        Action SuccessChooseName = null;

        // Personnal Data
        Action<PlayerDataPacket> OnPlayerData = null;
        Action<PlayerDataPacket> OnOtherPlayerData = null;

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
            RegisterPacketsSendOnly();
            RegisterPackets();
            ClientNetworkManager.Connect();
        }

        void RegisterPacketsSendOnly()
        {
            ClientNetworkManager.RegisterHandler<ConnectPacket>();
            ClientNetworkManager.RegisterHandler<ChooseNamePacket>();
            ClientNetworkManager.RegisterHandler<RequestPlayerData>();
        }

        void RegisterPackets()
        {
            ClientNetworkManager.RegisterHandler<ValidateConnectionPacket>((packet) => {
                if (packet.tokenValid)
                {
                    _connectedToServer = true;
                    if (onSuccess != null)
                    {
                        onSuccess();
                    }
                } else
                {
                    if(onFailure != null)
                    {
                        onFailure(packet.reasonInvalid);
                    }
                }
            });
            ClientNetworkManager.RegisterHandler<ResponseChooseNamePacket>((packet) => {
                if (packet.ok)
                {
                    SuccessChooseName();
                }
                else
                {
                    FailureChooseName(packet.reasonInvalid);
                }
            });
            ClientNetworkManager.RegisterHandler<PlayerDataPacket>((packet)=> {
                if (packet.personnalData)
                {
                    OnPlayerData(packet);
                }
                else
                {
                    OnOtherPlayerData(packet);
                }
            });
        }

        // Update is called once per frame
        void Update()
        {
            ClientNetworkManager.UpdateClient();
        }

        public void SignIn(string email, string password, Action<Exception> onFailure = null, Action onSuccess = null)
        {
            if (ClientNetworkManager.Connected)
            {
                this.onSuccess = onSuccess;
                cognito.TrySignInRequest(email, password,
                    onFailure,
                    (token, refreshToken) =>
                    {
                        ClientNetworkManager.SendPacket(
                                new ConnectPacket()
                                {
                                    token = token
                                }
                            );
                        PlayerPrefs.SetString("RefreshToken", refreshToken);
                    }
                );
            } else
            {
                onFailure(new Exception("Not connected to the server, please wait. if this error is persistent," +
                    " your internet connection may be down or the server is not available."));
            }
        }
        
        //Version which will use the refreshToken saved in the playerPref
        public void SignIn(Action<byte> onFailure = null, Action onSuccess = null)
        {
            if (ClientNetworkManager.Connected)
            {
                this.onSuccess = onSuccess;
                this.onFailure = onFailure;
                cognito.TrySignInRequestRefreshToken(PlayerPrefs.GetString("RefreshToken"),
                    (Exception) => onFailure((byte)1),
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
            }
            else
            {
                onFailure(0); // Not connected yet 
            }
        }

        public void SignUp(string email, string password, Action<Exception> onFailure = null, Action onSuccess = null)
        {
            cognito.TrySignUpRequest(email, password,
                onFailure,
                onSuccess
            );
        }

        public void ChangeUsername(string name, Action<byte> onFailure = null, Action onSuccess = null)
        {
            FailureChooseName = onFailure;
            SuccessChooseName = onSuccess;
            ClientNetworkManager.SendPacket(new ChooseNamePacket
            {
                name = name
            });
        }

        public void GetPlayerData(Action<PlayerDataPacket> onPlayerData)
        {
            OnPlayerData = onPlayerData;
            ClientNetworkManager.SendPacket(new RequestPlayerData());
        }
    }
}
