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
    public struct ConnectPacket : IPacket
    {
        public string token;
        public void Deserialize(BinaryReader reader)
        {
            this.token = reader.ReadString();
        }

        public byte OpCode()
        {
            return 1;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.token);
        }
    }
}