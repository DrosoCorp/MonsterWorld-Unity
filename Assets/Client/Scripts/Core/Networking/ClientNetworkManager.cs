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
        public static bool Connected
        {
            get
            {
                return connected;
            }
        }
        private static bool connected = false;
        protected delegate void NetworkMessageDelegate(ArraySegment<byte> bytes);
        private static Telepathy.Client client;
        private static List<Action> loadActions = new List<Action>();
        /// <summary>
        /// The registered network message handlers.
        /// </summary>
        static readonly Dictionary<byte, NetworkMessageDelegate> handlers = new Dictionary<byte, NetworkMessageDelegate>();
        static readonly Dictionary<byte, byte[]> writebuffers = new Dictionary<byte, byte[]>();

        public static void Init()
        {
            client = new Telepathy.Client(4096);
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
        public static void RegisterHandler<T>(Action<T> handler = null) where T : struct, IPacket
        {
            if (handler == null)
            {
                handler = (a) => { };
            }
            NetworkMessageDelegate del = (bytes) =>
            {
                T packet = default;
                packet.Deserialize(new BinaryReader(new MemoryStream(bytes.Array, bytes.Offset + 1, bytes.Count - 1, false)));
                handler(packet);
            };
            T p = default;
            handlers.Add(p.OpCode(), del);
            writebuffers.Add(p.OpCode(), new byte[4096]);
        }

        protected static void HandlePacket(ArraySegment<byte> bytes)
        {
            handlers[bytes.Array[bytes.Offset]](bytes);
        }

        public static void UpdateClient()
        {
            client.Tick(100);
        }

        public static void Connect()
        {
            client.Connect("localhost", 1337);
        }

        // Add function to call when the game finished to load the network
        public static void ConnectCallBack(Action load)
        {
            if(!connected)
            {
                ClientNetworkManager.loadActions.Add(load);
            } else
            {
                load();
            }
        }

        private static void HandleConnection()
        {
            connected = true;
            foreach(Action load in loadActions)
            {
                load();
            }
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
