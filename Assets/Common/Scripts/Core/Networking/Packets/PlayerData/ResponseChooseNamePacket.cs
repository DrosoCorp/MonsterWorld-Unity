//-----------------------------------------------------------------
// File:         ResponseChooseNamePacket.cs
// Description:  Packet for asking a name change
// Module:       Network.Client
// Author:       Thomas Hervé
// Date:         13/04/2021
//-----------------------------------------------------------------
using System.IO;

namespace MonsterWorld.Unity.Network
{
    public struct ResponseChooseNamePacket : IPacket
    {
        public bool ok;
        public void Deserialize(BinaryReader reader)
        {
            this.ok = reader.ReadBoolean();
        }

        public byte OpCode()
        {
            return 4;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.ok);
        }
    }
}