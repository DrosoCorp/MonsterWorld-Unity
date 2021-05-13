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
    public struct ConnectionResponsePacket : IPacket
    {
        public enum ResponseType : byte
        {
            Success = 0,
            InvalidToken = 1,
            RequestPlayerCreation = 2
        }

        public byte OpCode => 1;

        public ResponseType responseType;

        public void Deserialize(BinaryReader reader)
        {
            responseType = (ResponseType) reader.ReadByte();
        }

        public void Serialize(BinaryWriter writer)
        { 
            writer.Write((byte) responseType);
        }
    }
}