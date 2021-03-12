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
        protected static ClientNetworkManager manager;
        // Handles network messages on client and server
        protected delegate void NetworkMessageDelegate(IPacket packet);

        /// <summary>
        /// The registered network message handlers.
        /// </summary>
        static readonly Dictionary<byte, (NetworkMessageDelegate, IPacket)> handlers = new Dictionary<byte, (NetworkMessageDelegate, IPacket)>();
        static Telepathy.Client client;
        static Action load;

        public static void Init()
        {
            client = new Telepathy.Client(512);
            client.OnConnected = () => HandleConnection();
            client.OnData = (message) => HandlePacket(message);
            client.OnDisconnected = () => HandleDisconnection();
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

        // static methods, due to the singleton pattern we have to put those in all NetworkManager class
        public static ClientNetworkManager GetManager()
        {
            if (manager == null)
            {
                manager = new ClientNetworkManager();
            }
            return manager;
        }

        /// <summary>
        /// This function register an handler for a packet
        /// </summary>
        public static void _RegisterHandler<T>(Action<T> handler, IPacket packet) where T : struct, IPacket
        {
            NetworkMessageDelegate del = (a) =>
            {
                handler((T)a);
            };
            handlers[packet.OpCode()] = (del, packet);
        }

        public static void RegisterHandler<T>(Action<T> handler) where T : struct, IPacket
        {
            IPacket p = new T() { };
            _RegisterHandler(handler, p);
        }

        public static void HandlePacket(ArraySegment<byte> bytes)
        {
            var handler = handlers[bytes.Array[0]];
            byte[] data = new byte[bytes.Array.Length - 1];
            System.Buffer.BlockCopy(bytes.Array, 1, data, 0, data.Length);
            handler.Item2.Deserialize(new BinaryReader(new MemoryStream(data)));
            handler.Item1(handlers[bytes.Array[0]].Item2);
        }

        //Send packet
        public static void SendPacket(IPacket packet)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            packet.Serialize(writer);
            byte[] data = stream.ToArray();
            byte[] array = new byte[data.Length + 1];
            array[0] = packet.OpCode();
            System.Buffer.BlockCopy(data, 0, array, 1, data.Length);
            ArraySegment<byte> bytes = new ArraySegment<byte>(array);
            client.Send(bytes);
        }
    }
}
