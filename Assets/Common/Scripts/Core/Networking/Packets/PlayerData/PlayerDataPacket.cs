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
    public struct PlayerDataPacket : IPacket
    {
        public byte OpCode => 5;

        public bool isLocalPlayer;
        public string displayName;
        
        public void Deserialize(BinaryReader reader)
        {
            isLocalPlayer = reader.ReadBoolean();
            displayName = reader.ReadString();
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(isLocalPlayer);
            writer.Write(displayName);
        }
    }
}