//-----------------------------------------------------------------
// File:         ConnectPacket.cs
// Description:  Packet for asking a connection to the server
// Module:       Network.Client
// Author:       Thomas Hervé
// Date:         30/03/2021
//-----------------------------------------------------------------
using System.IO;

namespace MonsterWorld.Unity.Network
{
    public struct ConnectionPacket : IPacket
    {
        public byte OpCode => 0;

        public string token;

        public void Deserialize(BinaryReader reader)
        {
            token = reader.ReadString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(token);
        }
    }
}