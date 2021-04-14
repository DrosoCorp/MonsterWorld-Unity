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
    public struct PlayerData : IPacket
    {
        public bool personnalData; // True if the player receive this packet is the player described in this packet
        public string playerName;
        
        public void Deserialize(BinaryReader reader)
        {
            personnalData = reader.ReadBoolean();
            playerName = reader.ReadString();
        }

        public byte OpCode()
        {
            return 6;
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(personnalData);
            writer.Write(playerName);
        }
    }
}