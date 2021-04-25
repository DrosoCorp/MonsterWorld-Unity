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
        public static Dictionary<int, Guid> connectionIdToUIDWithoutName = new Dictionary<int, Guid>();

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
                        ServerNetworkManager.SendPacket(new ValidateConnectionPacket() { tokenValid = false, reasonInvalid = 1}, connectionId);
                    },
                    async (uid) =>
                    {
                        Guid guid = new Guid(uid);
                        // Check player exist in database
                        bool userExist = await PlayerDatabase.LoadUser(guid);
                        if (!userExist)
                        {
                            // You need to send an username to the server !
                            if (!connectionIdToUIDWithoutName.ContainsKey(connectionId))
                            {
                                connectionIdToUIDWithoutName.Add(connectionId, guid);
                            }
                            ServerNetworkManager.SendPacket(new ValidateConnectionPacket() { tokenValid = false, reasonInvalid = 2 }, connectionId);
                        }
                        else
                        {
                            connectionIdToUID.Add(connectionId, guid);
                            ServerNetworkManager.SendPacket(new ValidateConnectionPacket() { tokenValid = true }, connectionId);
                        }
                    });
            });
            ServerNetworkManager.RegisterHandler<ChooseNamePacket>(async (packet, connectionId) =>
            {
                if (UserConnected(connectionId))
                {
                    // A user is connected and want to change his username
                    ServerNetworkManager.SendPacket(new ResponseChooseNamePacket()
                    {
                        ok = await PlayerDatabase.UpdateUser(connectionIdToUID[connectionId], packet.name)
                    }, connectionId);
                }
                else
                { // The user have been check in cognito already
                    if (connectionIdToUIDWithoutName.ContainsKey(connectionId)) {
                        Guid uid = connectionIdToUIDWithoutName[connectionId];
                        bool valid = await PlayerDatabase.UpdateUser(uid, packet.name); // UpdateUser can also create users
                        if (!valid)
                        {
                            ServerNetworkManager.SendPacket(new ValidateConnectionPacket() { tokenValid = false, reasonInvalid = 1 }, connectionId);
                        }
                        else
                        {
                            // Swap dictionnaries
                            connectionIdToUID[connectionId] = connectionIdToUIDWithoutName[connectionId];
                            connectionIdToUIDWithoutName.Remove(connectionId);
                            // The entry have been created, return the validation
                            ServerNetworkManager.SendPacket(new ResponseChooseNamePacket()
                            {
                                ok = true
                            }, connectionId);
                        }
                    } else
                    {
                        // You can't change your name without being connected previously
                        ServerNetworkManager.SendPacket(new ResponseChooseNamePacket() { ok = false }, connectionId);
                    }
                }
            });
            ServerNetworkManager.RegisterHandler<RequestPlayerData>(async (packet, connectionId) =>
            {
                if (UserConnected(connectionId))
                {
                    PlayerStruct player = PlayerDatabase.GetPlayerData(GetUID(connectionId));
                    ServerNetworkManager.SendPacket(new PlayerData()
                    {
                        personnalData = true, // The packet RequestPlayerData is send only to retrieve the data of the player which send this packet
                        playerName = player.name
                    }, connectionId); ;
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
            } else if (connectionIdToUIDWithoutName.ContainsKey(connection))
            {
                connectionIdToUIDWithoutName.Remove(connection);
            }
        }
    }
}
