//-----------------------------------------------------------------
// File:         HelpCommand.cs
// Description:  Command to display an help message
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         14/05/2021
//-----------------------------------------------------------------
using MonsterWorld.Unity.Network.Server.Cli;
using System;
using UnityEngine;

namespace MonsterWorld.Unity.Network.Server
{
    public class HelpCommand : Command
    {
        public override string[] Alias => new string[] { "Help", "help" };

        public override void Execute(params string[] arguments)
        {
            string t = "!";
            if(arguments.Length > 1)
            {
                t = arguments[1];
            }
            Debug.unityLogger.Log("Nouveau message de log "+t);
            ServerConsole.Print("No commands are currently available, but the command system exist and is fonctionnal \\O/");
        }
    }
}
