//-----------------------------------------------------------------
// File:         ChooseNamePacket.cs
// Description:  Packet for asking a name change
// Module:       Network
// Author:       Thomas Hervé
// Date:         13/04/2021
//-----------------------------------------------------------------
using System.IO;

namespace MonsterWorld.Unity.Network
{
    public struct ChooseNamePacket : IPacket
    {
        public string name;
        public void Deserialize(BinaryReader reader)
        {
            this.name = reader.ReadString();
        }

        public byte OpCode()
        {
            return 3;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(this.name);
        }
    }
}