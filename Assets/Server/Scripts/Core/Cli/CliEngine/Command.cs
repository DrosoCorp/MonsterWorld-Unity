//-----------------------------------------------------------------
// File:         Command.cs
// Description:  Abstract class for command
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         14/05/2021
//-----------------------------------------------------------------
namespace MonsterWorld.Unity.Network.Server.Cli
{
    public abstract class Command
    {
        public abstract string[] Alias { get; }

        public abstract void Execute(params string[] arguments);

    }
}
