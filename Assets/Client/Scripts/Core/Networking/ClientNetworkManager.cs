//-----------------------------------------------------------------
// File:         ClientNetworkManager.cs
// Description:  Implementation of the NetworkManager for the client
// Module:       Client.Server
// Author:       Thomas Hervé
// Date:         27/02/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MonsterWorld.Unity.Network.Client
{
    public class ClientNetworkManager 
    {
        // Handles network messages on client and server
        public static bool Connected { get; private set; } = false;

        public delegate void PacketHandlerDelegate<T>(ref T packet);
        private delegate void NetworkMessageDelegate(ArraySegment<byte> bytes);

        private static Telepathy.Client client;
        /// <summary>
        /// The registered network message handlers.
        /// </summary>
        static readonly Dictionary<byte, NetworkMessageDelegate> handlers = new Dictionary<byte, NetworkMessageDelegate>();
        static readonly Dictionary<byte, byte[]> writebuffers = new Dictionary<byte, byte[]>();

        public static Action OnConnected;

        public static PacketHandlerDelegate<ConnectionResponsePacket> OnConnectionResponsePacket;
        public static PacketHandlerDelegate<PlayerCreationResponsePacket> OnPlayerCreationResponsePacket;
        public static PacketHandlerDelegate<PlayerDataPacket> OnPlayerDataPacket;

        public static void Init()
        {
            client = new Telepathy.Client(4096);
            client.OnConnected = () => HandleConnection();
            client.OnData = (message) => HandlePacket(message);
            client.OnDisconnected = () => HandleDisconnection();

            RegisterPackets();
        }

        private static void RegisterPackets()
        {
            RegisterPacket<ConnectionPacket>();
            RegisterPacket<PlayerCreationPacket>();
            RegisterPacket<RequestPlayerDataPacket>();

            RegisterHandler((ref ConnectionResponsePacket packet) => OnConnectionResponsePacket(ref packet));
            RegisterHandler((ref PlayerCreationResponsePacket packet) => OnPlayerCreationResponsePacket(ref packet));
            RegisterHandler((ref PlayerDataPacket packet) => OnPlayerDataPacket(ref packet));
        }

        public static void Stop()
        {
            client.Disconnect();
        }

        //Send packet
        static protected ArraySegment<byte> GetBytesFromPacket<T>(ref T packet) where T : struct, IPacket
        {
            var writeBuffer = writebuffers[packet.OpCode];
            MemoryStream stream = new MemoryStream(writeBuffer, true);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(packet.OpCode);
            packet.Serialize(writer);
            var segment = new ArraySegment<byte>(writeBuffer, 0, (int)stream.Length);
            return segment;
        }

        public static void RegisterPacket<T>() where T : struct, IPacket
        {
            T packet = default;
            writebuffers.Add(packet.OpCode, new byte[4096]);
        }

        /// <summary>
        /// This function register an handler for a packet
        /// </summary>
        public static void RegisterHandler<T>(PacketHandlerDelegate<T> handler) where T : struct, IPacket
        {
            if (handler == null)
            {
                return;
            }
            NetworkMessageDelegate del = (bytes) =>
            {
                T packet = default;
                packet.Deserialize(new BinaryReader(new MemoryStream(bytes.Array, bytes.Offset + 1, bytes.Count - 1, false)));
                handler(ref packet);
            };
            T packet = default;
            handlers.Add(packet.OpCode, del);
        }

        protected static void HandlePacket(ArraySegment<byte> bytes)
        {
            if (handlers.TryGetValue(bytes.Array[bytes.Offset], out NetworkMessageDelegate handler))
            {
                handler(bytes);
            }
        }

        public static void UpdateClient()
        {
            client.Tick(100);
        }

        public static void Connect(string ip, int port)
        {
            client.Connect(ip, port);
        }

        private static void HandleConnection()
        {
            Connected = true;
            if (OnConnected != null) OnConnected();
        }

        private static void HandleDisconnection()
        {

        }

        public static void SendPacket<T>(ref T packet) where T : struct, IPacket
        {
            ArraySegment<byte> bytes = GetBytesFromPacket(ref packet);
            client.Send(bytes);
        }
    }
}
