
//-----------------------------------------------------------------
// File:         IConsole.cs
// Description:  Interface for the console which handle admin commands
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         14/05/2021
//-----------------------------------------------------------------
namespace MonsterWorld.Unity.Network.Server.Cli
{
    public interface IConsole
    {
        /// <summary>
        /// Run the CLI after configurate it
        /// </summary>
        public void Run();

        /// <summary>
        /// Add a command 
        /// </summary> 
        public void AddCommand(Command command);
    }
}