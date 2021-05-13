//-----------------------------------------------------------------
// File:         PlayerCreationResponsePacket.cs
// Description:  Packet for asking a name change
// Module:       Network.Client
// Author:       Thomas Hervé
// Date:         13/04/2021
//-----------------------------------------------------------------
using System.IO;

namespace MonsterWorld.Unity.Network
{
    public struct PlayerCreationResponsePacket : IPacket
    {
        public enum ResponseType : byte
        {
            Success = 0,
            ClientNotAuthenticated = 1,
            PlayerAlreadyExists = 2,
            UsernameAlreadyTaken = 3,
            InvalidPayload = 4
        }

        public byte OpCode => 3;

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