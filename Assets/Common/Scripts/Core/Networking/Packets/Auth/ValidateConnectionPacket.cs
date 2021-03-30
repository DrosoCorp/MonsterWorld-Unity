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
    public struct ValidateConnectionPacket : IPacket
    {
        public bool tokenValid;
        public void Deserialize(BinaryReader reader)
        {
            this.tokenValid = reader.ReadBoolean();
        }

        public byte OpCode()
        {
            return 2;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.tokenValid);
        }
    }
}