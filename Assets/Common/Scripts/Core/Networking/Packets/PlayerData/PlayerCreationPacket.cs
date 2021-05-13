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
    public struct PlayerCreationPacket : IPacket
    {
        public byte OpCode => 2;

        public string name;

        public void Deserialize(BinaryReader reader)
        {
            name = reader.ReadString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(name);
        }
    }
}