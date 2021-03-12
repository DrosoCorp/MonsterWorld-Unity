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

namespace MonsterWorld.Unity.Network.Server
{
    public class ServerNetworkManager
    {
        protected static ServerNetworkManager manager;
        // Handles network messages on client and server
        protected delegate void NetworkMessageDelegate(IPacket packet, int connectionID);

        /// <summary>
        /// The registered network message handlers.
        /// </summary>
        static readonly Dictionary<byte, (NetworkMessageDelegate, IPacket)> handlers = new Dictionary<byte, (NetworkMessageDelegate, IPacket)>();
        static Telepathy.Server server;

        public static void Init()
        {
            server = new Telepathy.Server(512);
            server.OnConnected = (connectionId) => HandleConnection(connectionId);
            server.OnData = (connectionId, message) => HandlePacket(message, connectionId);
            server.OnDisconnected = (connectionId) => HandleDisconnection(connectionId);
            server.Start(1337);
        }

        static public void UpdateServer()
        {
            server.Tick(100);
        }

        private static void HandleConnection(int connectionId)
        {
            
        }

        private static void HandleDisconnection(int connectionId)
        {

        }

        /// <summary>
        /// This function register an handler for a packet
        /// </summary>
        private static void _RegisterHandler<T>(Action<T, int> handler, IPacket packet) where T : struct, IPacket
        {
            NetworkMessageDelegate del = (a, connectionID) =>
            {
                handler((T)a, connectionID);
            };
            handlers[packet.OpCode()] = (del, packet);
        }

        public static void RegisterHandler<T>(Action<T, int> handler) where T : struct, IPacket
        {
            IPacket p = new T() { };
            _RegisterHandler(handler, p);
        }

        private static void HandlePacket(ArraySegment<byte> bytes, int connectionID)
        {
            var handler = handlers[bytes.Array[0]];
            byte[] data = new byte[bytes.Array.Length - 1];
            System.Buffer.BlockCopy(bytes.Array, 1, data, 0, data.Length);
            handler.Item2.Deserialize(new BinaryReader(new MemoryStream(data)));
            handler.Item1(handlers[bytes.Array[0]].Item2, connectionID);
        }

        //Send packet
        static private ArraySegment<byte> GetBytesFromPacket(IPacket packet)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            packet.Serialize(writer);
            byte[] data = stream.ToArray();
            byte[] array = new byte[data.Length + 1];
            array[0] = packet.OpCode();
            System.Buffer.BlockCopy(data, 0, array, 1, data.Length);
            return new ArraySegment<byte>(array);
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

    }
}
