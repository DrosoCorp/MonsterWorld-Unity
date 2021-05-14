//-----------------------------------------------------------------
// File:         Playerdata.cs
// Description:  Packet for transmitting the data of a specific player
// Module:       Network
// Author:       Thomas Hervé
// Date:         14/04/2021
//-----------------------------------------------------------------
using System.IO;

namespace MonsterWorld.Unity.Network
{
    public struct RequestPlayerDataPacket : IPacket
    {
        public byte OpCode => 4;
        
        public void Deserialize(BinaryReader reader) {}

        public void Serialize(BinaryWriter writer) {}
    }
}