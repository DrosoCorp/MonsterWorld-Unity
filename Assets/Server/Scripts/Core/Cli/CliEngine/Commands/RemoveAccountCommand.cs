//-----------------------------------------------------------------
// File:         RemoveAccountCommand.cs
// Description:  Command to run to remove an account in databse
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         14/05/2021
//-----------------------------------------------------------------
using MonsterWorld.Unity.Network.Server.Cli;
using System;

namespace MonsterWorld.Unity.Network.Server
{
    public class RemoveAccountCommand : Command
    {
        public override string[] Alias => new string[] { "Remove", "remove" };

        public override void Execute(params string[] arguments)
        {
            
        }
    }
}