//-----------------------------------------------------------------
// File:         BattleCommand.cs
// Description:  Describe a battle command
// Module:       BattleSystem
// Author:       Noé Masse
// Date:         27/02/2021
//-----------------------------------------------------------------
namespace MonsterWorld.Unity.BattleSystem
{
    public enum BattleCommandType
    {
        UseMove = 0,
        Swap = 1,
        UseItem = 2,
        Surrend = 3
    }

    public struct BattleCommand
    {
        // Total size: 20 bytes
        public int actorId;
        public BattleCommandType type;
        // Additional values
        public int additionalValue0;
        public int additionalValue1;
        public int additionalValue2;
        public int additionalValue3;
    }
}