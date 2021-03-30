//-----------------------------------------------------------------
// File:         ServerAuth.cs
// Description:  Class that initialize the network and offer methods to receive connections
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         30/03/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using AWS;
using UnityEngine;

namespace MonsterWorld.Unity.Network.Server
{
    public class ServerAuth : MonoBehaviour
    {
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

        public static Dictionary<int, Guid> connectionIdToUID = new Dictionary<int, Guid>();

        // Start is called before the first frame update
        void Start()
        {
            ServerNetworkManager.Init();
            ServerNetworkManager.RegisterHandler<ValidateConnectionPacket>();
            ServerNetworkManager.RegisterHandler<ConnectPacket>((paquet, connectionId) => {
                cognito.GetUser(paquet.token,
                    (exception) =>
                    {
                        ServerNetworkManager.SendPacket(new ValidateConnectionPacket() { tokenValid = false }, connectionId);
                    },
                    (uid) =>
                    {
                        connectionIdToUID.Add(connectionId, new Guid(uid));
                        ServerNetworkManager.SendPacket(new ValidateConnectionPacket() { tokenValid = true }, connectionId);
                        Debug.Log($"User {uid} connected.");
                    });
            });
        }

        // Update is called once per frame
        void Update()
        {
            ServerNetworkManager.UpdateServer();
        }
    }
}
