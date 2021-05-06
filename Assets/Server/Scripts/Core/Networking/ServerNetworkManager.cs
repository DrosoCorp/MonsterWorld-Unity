//-----------------------------------------------------------------
// File:         ServerNetworkManager.cs
// Description:  Implementation of the NetworkManager for the server
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         27/02/2021
//-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;

namespace MonsterWorld.Unity.Network.Server
{
    public class ServerNetworkManager
    {
        // Handles network messages on client and server
        protected delegate void NetworkMessageDelegate(ArraySegment<byte> bytes, int connectionID);
        protected static ServerNetworkManager manager;
        static Telepathy.Server server;
        /// <summary>
        /// The registered network message handlers.
        /// </summary>
        static readonly Dictionary<byte, NetworkMessageDelegate> handlers = new Dictionary<byte, NetworkMessageDelegate>();
        static readonly Dictionary<byte, byte[]> writebuffers = new Dictionary<byte, byte[]>();

        // The server instance for callbacks
        static ServerAuth serverInstance;

        public static void Init()
        {
            server = new Telepathy.Server(4096);
            server.OnConnected = (connectionId) => HandleConnection(connectionId);
            server.OnData = (connectionId, message) => HandlePacket(message, connectionId);
            server.OnDisconnected = (connectionId) => HandleDisconnection(connectionId);
            server.Start(1337);
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
        public static void RegisterHandler<T>(Action<T, int> handler = null) where T : struct, IPacket
        {
            if(handler == null)
            {
                handler = (a, b) => { };
            }
            NetworkMessageDelegate del = (bytes, connectionID) =>
            {
                T packet = default;
                packet.Deserialize(new BinaryReader(new MemoryStream(bytes.Array, bytes.Offset + 1, bytes.Count - 1, false)));
                handler(packet, connectionID);
            };
            T p = default;
            handlers.Add(p.OpCode(), del);
            writebuffers.Add(p.OpCode(), new byte[4096]);
        }

        protected static void HandlePacket(ArraySegment<byte> bytes, int connectionID)
        {
            handlers[bytes.Array[bytes.Offset]](bytes, connectionID);
        }

        static public void UpdateServer()
        {
            server.Tick(100);
        }

        public static void addServerInstance(ServerAuth s)
        {
            serverInstance = s;
        }

        private static void HandleConnection(int connectionId)
        {
            var a = 1;
        }

        private static void HandleDisconnection(int connectionId)
        {
            serverInstance.Disconnection(connectionId);
        }

        static public void SendPacket(IPacket packet, int[] connectionList)
        {
            ArraySegment<byte> bytes = GetBytesFromPacket(packet);
            foreach (int connectionId in connectionList)
            {
                server.Send(connectionId, bytes);
            }
        }

        static public void SendPacket(IPacket packet, int connectionId)
        {
            ArraySegment<byte> bytes = GetBytesFromPacket(packet);
            server.Send(connectionId, bytes);
        }

        static public void Stop()
        {
            server.Stop();
        }

    }
}
