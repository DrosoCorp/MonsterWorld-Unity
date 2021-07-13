//-----------------------------------------------------------------
// File:         ServerCLIService.cs
// Description:  Class that initialize the network and offer methods to receive connections
// Module:       Network.Server
// Author:       Thomas Hervé
// Date:         14/05/2021
//-----------------------------------------------------------------
using System.Threading;
using MonsterWorld.Unity.Network.Server.Cli;

namespace MonsterWorld.Unity.Network.Server
{
    public class ServerCLIService : Service<ServerCLIService>
    {
        protected override void Dispose()
        {
            
        }

        protected override void Initialize()
        {
            Thread t = new Thread(new ThreadStart(Cli));
            t.Start();
        }

        private void Cli()
        {
            IConsole console = new ServerConsole();
            console.AddCommand(new HelpCommand());
            console.AddCommand(new RemoveAccountCommand());
            console.Run();
        }

    }
}