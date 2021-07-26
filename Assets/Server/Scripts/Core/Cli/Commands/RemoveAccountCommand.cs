//-----------------------------------------------------------------
// File:         RemoveAccountCommand.cs
// Description:  Command to run to remove an account in databse
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         14/05/2021
//-----------------------------------------------------------------
using MonsterWorld.Unity.Network.Server.Cli;

namespace MonsterWorld.Unity.Network.Server
{
    public class RemoveAccountCommand : Command
    {
        public override string[] Alias => new string[] { "Remove", "remove" };

        public async override void Execute(params string[] arguments)
        {
            if(arguments.Length < 2)
            {
                ServerConsole.Print("This command need to take the username of the targeted user as first argument");
                return;
            }
            if (await PlayerDatabase.RemoveUser(arguments[1]))
            {
                ServerConsole.Print($"The user {arguments[1]} has been successfully deleted");
            }
            else
            {
                ServerConsole.Print($"The user {arguments[1]} doesn't have any entry in the database");
            }
        }
    }
}