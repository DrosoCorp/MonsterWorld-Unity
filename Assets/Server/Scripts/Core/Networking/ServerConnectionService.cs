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
    public class ServerConnectionService : Service<ServerConnectionService>
    {
        private ICognito _cognito;

        public static Dictionary<int, Guid> connectionIdToUID = new Dictionary<int, Guid>();
        public static Dictionary<int, Guid> pendingPlayerCreationRequests = new Dictionary<int, Guid>();

        public int port = 1375;

        protected override void Initialize()
        {
            _cognito = new Cognito();
            ServerNetworkManager.Init(port);

            RegisterPackets();

            ServerNetworkManager.OnClientDisconnected += OnClientDisconnected;
        }

        protected override void Dispose()
        {
            ServerNetworkManager.Stop();
        }

        void RegisterPackets()
        {
            ServerNetworkManager.RegisterPacket<ConnectionResponsePacket>();
            ServerNetworkManager.RegisterPacket<PlayerCreationResponsePacket>();
            ServerNetworkManager.RegisterPacket<PlayerDataPacket>();

            ServerNetworkManager.RegisterHandler<ConnectionPacket>(OnConnectionPacket);
            ServerNetworkManager.RegisterHandler<PlayerCreationPacket>(OnPlayerCreationPacket);
        }

        // Update is called once per frame
        void Update()
        {
            ServerNetworkManager.UpdateServer();
        }

        public bool IsPlayerConnected(int connectionId)
        {
            return connectionIdToUID.ContainsKey(connectionId);
        }

        public static Guid GetUID(int connectionId)
        {
            return connectionIdToUID[connectionId];
        }

        #region Handlers
        private void OnConnectionPacket(ConnectionPacket packet, int connectionId)
        {
            _cognito.GetUser(packet.token,
            // On Authentication Failed
            (exception) =>
            {
                var response = new ConnectionResponsePacket()
                {
                    responseType = ConnectionResponsePacket.ResponseType.InvalidToken
                };
                ServerNetworkManager.SendPacket(connectionId, response);
            },
            // On Authentication Success
            async (uid) =>
            {
                Guid guid = new Guid(uid);
                bool playerExists = await PlayerDatabase.PlayerConnection(guid);

                if (!playerExists)
                {
                    // Request player creation
                    pendingPlayerCreationRequests.Add(connectionId, guid);

                    var response = new ConnectionResponsePacket()
                    {
                        responseType = ConnectionResponsePacket.ResponseType.RequestPlayerCreation
                    };
                    ServerNetworkManager.SendPacket(connectionId, response);
                }
                else
                {
                    // Connection success
                    connectionIdToUID.Add(connectionId, guid);

                    var response = new ConnectionResponsePacket()
                    {
                        responseType = ConnectionResponsePacket.ResponseType.Success
                    };

                    var playerDataPacket = new PlayerDataPacket()
                    {
                        isLocalPlayer = true,
                        displayName = PlayerDatabase.GetPlayerData(guid).name
                    };

                    ServerNetworkManager.SendPacket(connectionId, response);
                    ServerNetworkManager.SendPacket(connectionId, playerDataPacket);
                }
            });
        }

        private async void OnPlayerCreationPacket(PlayerCreationPacket packet, int connectionId)
        {
            PlayerCreationResponsePacket response;
            if (IsPlayerConnected(connectionId))
            {
                // The user already has a player data
                response.responseType = PlayerCreationResponsePacket.ResponseType.PlayerAlreadyExists;
            }
            else
            { 
                if (pendingPlayerCreationRequests.TryGetValue(connectionId, out Guid uid))
                {
                    // The client is connected and server is waiting for player creation data
                    int databaseResponse = await ServerDatabase.UsernameAvailable(uid.ToString(), packet.name);
                    bool isUsernameAvailable = databaseResponse == 0;

                    if (!isUsernameAvailable)
                    {
                        response.responseType = PlayerCreationResponsePacket.ResponseType.UsernameAlreadyTaken;
                    }
                    else
                    {
                        PlayerData playerData = PlayerDatabase.CreatePlayerData(pendingPlayerCreationRequests[connectionId], packet.name);
                        PlayerDatabase.CreateUser(pendingPlayerCreationRequests[connectionId], playerData); // Create the user

                        // Swap dictionnaries
                        connectionIdToUID[connectionId] = uid;
                        pendingPlayerCreationRequests.Remove(connectionId);

                        response.responseType = PlayerCreationResponsePacket.ResponseType.Success;
                    }
                }
                else
                {
                    response.responseType = PlayerCreationResponsePacket.ResponseType.ClientNotAuthenticated;
                }
            }
            ServerNetworkManager.SendPacket(connectionId, response);
        }

        public void OnClientDisconnected(int connectionId)
        {
            if (connectionIdToUID.ContainsKey(connectionId))
            {
                connectionIdToUID.Remove(connectionId);
            }
            else if (pendingPlayerCreationRequests.ContainsKey(connectionId))
            {
                pendingPlayerCreationRequests.Remove(connectionId);
            }
        }
        #endregion
    }
}
