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
            ServerNetworkManager.RegisterHandler<PlayerDataPacket>();
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
                        bool userExist = await PlayerDatabase.PlayerConnection(guid);
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
                    // return;
                    // A user is connected and want to change his username
                    Guid uid = connectionIdToUID[connectionId];
                    int result = await ServerDatabase.UsernameAvailable(uid.ToString(), packet.name);
                    bool ok = result == 0;
                    if (ok)
                    {
                        PlayerData p = PlayerDatabase.GetPlayerData(uid);
                        p.name = packet.name;
                        PlayerDatabase.MarkForUpdate(p);
                    }
                    ServerNetworkManager.SendPacket(new ResponseChooseNamePacket()
                    {
                        ok = ok,
                        reasonInvalid = (byte)result
                    }, connectionId);
                }
                else
                { // The user have been check in cognito already
                    if (connectionIdToUIDWithoutName.ContainsKey(connectionId)) {
                        Guid uid = connectionIdToUIDWithoutName[connectionId];
                        int valid = await ServerDatabase.UsernameAvailable(uid.ToString(), packet.name);
                        bool ok = valid == 0;
                        if (!ok)
                        {
                            ServerNetworkManager.SendPacket(new ValidateConnectionPacket() { tokenValid = false, reasonInvalid = (byte)valid }, connectionId);
                        }
                        else
                        {
                            PlayerData p = PlayerDatabase.GetEmptyPlayer(connectionIdToUIDWithoutName[connectionId], packet.name);
                            PlayerDatabase.CreateUser(connectionIdToUIDWithoutName[connectionId], p); // Create the user

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
            ServerNetworkManager.RegisterHandler<RequestPlayerData>((packet, connectionId) =>
            {
                if (UserConnected(connectionId))
                {
                    PlayerData player = PlayerDatabase.GetPlayerData(GetUID(connectionId));
                    ServerNetworkManager.SendPacket(new PlayerDataPacket()
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

        public static Guid GetUID(int connectionId)
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

        public void OnDestroy()
        {
            ServerNetworkManager.Stop();
        }
    }
}
