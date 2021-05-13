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
using UnityEngine;

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

        public static Action<int> OnClientDisconnected;

        public static void Init(int port)
        {
            server = new Telepathy.Server(4096);
            server.OnConnected = HandleConnection;
            server.OnData = HandlePacket;
            server.OnDisconnected = HandleDisconnection;
            server.Start(port);
        }

        //Send packet
        static protected ArraySegment<byte> GetBytesFromPacket(IPacket packet)
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
        public static void RegisterHandler<T>(Action<T, int> handler) where T : struct, IPacket
        {
            if(handler == null)
            {
                return;
            }
            NetworkMessageDelegate del = (bytes, connectionID) =>
            {
                T packet = default;
                packet.Deserialize(new BinaryReader(new MemoryStream(bytes.Array, bytes.Offset + 1, bytes.Count - 1, false)));
                handler(packet, connectionID);
            };
            T packet = default;
            handlers.Add(packet.OpCode, del);
            writebuffers.Add(packet.OpCode, new byte[4096]);
        }

        protected static void HandlePacket(int connectionID, ArraySegment<byte> bytes)
        {
            if (handlers.TryGetValue(bytes.Array[bytes.Offset], out NetworkMessageDelegate handler))
            {
                handler(bytes, connectionID);
            }
        }

        static public void UpdateServer()
        {
            server.Tick(100);
        }

        private static void HandleConnection(int connectionId)
        {
            Debug.Log("Connection : " + connectionId);
        }

        private static void HandleDisconnection(int connectionId)
        {
            OnClientDisconnected(connectionId);
        }

        static public void SendPacket(int[] connectionList, IPacket packet)
        {
            ArraySegment<byte> bytes = GetBytesFromPacket(packet);
            foreach (int connectionId in connectionList)
            {
                server.Send(connectionId, bytes);
            }
        }

        static public void SendPacket(int connectionId, IPacket packet)
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
