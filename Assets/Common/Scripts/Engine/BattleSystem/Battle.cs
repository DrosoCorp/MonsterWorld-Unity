//-----------------------------------------------------------------
// File:         BattleAction.cs
// Description:  Represent a result action that need to be processed
// Module:       BattleSystem
// Author:       Noé Masse
// Date:         27/02/2021
//-----------------------------------------------------------------
using Unity.Collections;

namespace MonsterWorld.Unity.BattleSystem
{
    public class Battle
    {
        private bool _IsStateValid = false;

        public bool IsValid => _IsStateValid;

        public void ComputeTurn(NativeList<BattleCommand> commandBuffer, NativeList<BattleAction> results)
        {
            results.Clear();

            for (int i = 0; i < commandBuffer.Length; i++)
            {

                var commandAction = new BattleAction();
                commandAction.type = BattleActionType.Command;
                commandAction.result = BattleActionResult.Success;

                var damageAction = new BattleAction();
                damageAction.type = BattleActionType.Damage;
                damageAction.result = BattleActionResult.Success;

                results.Add(commandAction);
                results.Add(damageAction);
            }
        }
    }
}

