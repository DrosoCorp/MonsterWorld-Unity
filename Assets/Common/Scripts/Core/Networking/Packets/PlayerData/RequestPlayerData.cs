//-----------------------------------------------------------------
// File:         RequestPlayerdata.cs
// Description:  Packet for asking the player data
// Module:       Network
// Author:       Thomas Herv�
// Date:         14/04/2021
//-----------------------------------------------------------------
using System.IO;

namespace MonsterWorld.Unity.Network
{
    public struct RequestPlayerData : IPacket
    {
        public void Deserialize(BinaryReader reader)
        {
            
        }

        public byte OpCode()
        {
            return 5;
        }

        public void Serialize(BinaryWriter writer)
        {
            
        }
    }
}