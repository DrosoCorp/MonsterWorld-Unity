//-----------------------------------------------------------------
// File:         Battle.cs
// Description:  Contains the battle state
// Module:       BattleSystem
// Author:       Noé Masse
// Date:         27/02/2021
//-----------------------------------------------------------------
namespace MonsterWorld.Unity.BattleSystem
{
    public enum BattleActionType : int
    {
        Command = 0,
        Damage = 1,
        Effect = 2,
        Knockout = 3
    }

    public enum BattleActionResult : int
    {
        Success = 0,
        Failure = 1,
        NoEffect = 2
    }

    public struct BattleAction
    {
        // Total size: 32 bytes
        public BattleActionType type;
        public BattleActionResult result;
        public int source;
        public int target;
        // Additional values
        public int additionalValue0;
        public int additionalValue1;
        public int additionalValue2;
        public int additionalValue3;
    }
}
