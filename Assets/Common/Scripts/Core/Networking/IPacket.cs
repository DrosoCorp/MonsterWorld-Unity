//-----------------------------------------------------------------
// File:         IPacket.cs
// Description:  Interface for packets
// Module:       Network
// Author:       Thomas Hervé
// Date:         27/02/2021
//-----------------------------------------------------------------
using System.IO;

namespace MonsterWorld.Unity.Network
{
    public interface IPacket
    {
        byte OpCode();

        void Deserialize(BinaryReader reader);

        void Serialize(BinaryWriter writer);
    }

}