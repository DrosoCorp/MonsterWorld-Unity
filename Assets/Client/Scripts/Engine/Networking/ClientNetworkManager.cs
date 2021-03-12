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

namespace MonsterWorld.Unity.Network.Client
{
    public class ClientNetworkManager 
    {
        // Handles network messages on client and server
        protected delegate void NetworkMessageDelegate(ArraySegment<byte> bytes);
        private static Telepathy.Client client;
        private static Action load;
        /// <summary>
        /// The registered network message handlers.
        /// </summary>
        static readonly Dictionary<byte, NetworkMessageDelegate> handlers = new Dictionary<byte, NetworkMessageDelegate>();
        static readonly Dictionary<byte, byte[]> writebuffers = new Dictionary<byte, byte[]>();

        public static void Init()
        {
            client = new Telepathy.Client(512);
            client.OnConnected = () => HandleConnection();
            client.OnData = (message) => HandlePacket(message);
            client.OnDisconnected = () => HandleDisconnection();
        }

        //Send packet
        static protected ArraySegment<byte> GetBytesFromPacket(IPacket packet)
        {
            var writeBuffer = writebuffers[packet.OpCode()];
            MemoryStream stream = new MemoryStream(writeBuffer, true);
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(packet.OpCode());
            packet.Serialize(writer);
            var segment = new ArraySegment<byte>(writeBuffer, 0, (int)stream.Length);
            return segment;
        }

        /// <summary>
        /// This function register an handler for a packet
        /// </summary>
        public static void RegisterHandler<T>(Action<T> handler) where T : struct, IPacket
        {
            NetworkMessageDelegate del = (bytes) =>
            {
                T packet = default;
                packet.Deserialize(new BinaryReader(new MemoryStream(bytes.Array, bytes.Offset + 1, bytes.Count - 1, false)));
                handler(packet);
            };
            T p = default;
            handlers.Add(p.OpCode(), del);
            writebuffers.Add(p.OpCode(), new byte[512]);
        }

        protected static void HandlePacket(ArraySegment<byte> bytes)
        {
            handlers[bytes.Array[bytes.Offset]](bytes);
        }

        public static void UpdateClient()
        {
            client.Tick(100);
        }

        public static void Connect(Action load)
        {
            client.Connect("localhost", 1337);
            ClientNetworkManager.load = load;
        }

        private static void HandleConnection()
        {
            load();
        }

        private static void HandleDisconnection()
        {

        }

        //Send packet
        public static void SendPacket(IPacket packet)
        {
            ArraySegment<byte> bytes = GetBytesFromPacket(packet);
            client.Send(bytes);
        }
    }
}
