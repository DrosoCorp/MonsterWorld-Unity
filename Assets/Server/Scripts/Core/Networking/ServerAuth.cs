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
            RegisterPacketsSendOnly();
            RegisterPackets();
        }

        void RegisterPacketsSendOnly()
        {
            ServerNetworkManager.RegisterHandler<ValidateConnectionPacket>();
            ServerNetworkManager.RegisterHandler<ResponseChooseNamePacket>();
            ServerNetworkManager.RegisterHandler<PlayerData>();
        }

        void RegisterPackets()
        {
            ServerNetworkManager.RegisterHandler<ConnectPacket>((packet, connectionId) => {
                cognito.GetUser(packet.token,
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
            ServerNetworkManager.RegisterHandler<ChooseNamePacket>(async (packet, connectionId) =>
            {
                if (UserConnected(connectionId))
                {
                    ServerNetworkManager.SendPacket(new ResponseChooseNamePacket()
                    {
                        ok = await ServerDatabase.SetUsername(GetUID(connectionId).ToString(), packet.name)
                    }, connectionId);
                }
            });
            ServerNetworkManager.RegisterHandler<RequestPlayerData>(async (packet, connectionId) =>
            {
                if (UserConnected(connectionId))
                {
                    ServerNetworkManager.SendPacket(new PlayerData()
                    {
                        personnalData = true,
                        playerName = await ServerDatabase.GetName(GetUID(connectionId).ToString())
                    }, connectionId);
                }
            });

        }

        // Update is called once per frame
        void Update()
        {
            ServerNetworkManager.UpdateServer();
        }

        // Utility
        private bool UserConnected(int connectionId)
        {
            return connectionIdToUID.ContainsKey(connectionId);
        }

        private Guid GetUID(int connectionId)
        {
            return connectionIdToUID[connectionId];
        }

        // Disconnection callback
        public void Disconnection(int connection)
        {
            if (connectionIdToUID.ContainsKey(connection))
            {
                connectionIdToUID.Remove(connection);
            }
        }
    }
}
