//-----------------------------------------------------------------
// File:         ClientAuth.cs
// Description:  Class that initialize the network and offer methods to connect
// Module:       Network.Client
// Author:       Thomas Hervé
// Date:         30/03/2021
//-----------------------------------------------------------------
using UnityEngine;
using AWS;
using System;
using System.Threading.Tasks;
using System.Collections;

namespace MonsterWorld.Unity.Network.Client
{
    public class ClientNetworkService : Service<ClientNetworkService>
    {
        private ICognito _cognito = null;
        public ICognito Authenticator
        {
            get
            {
                if (_cognito == null)
                    _cognito = new Cognito();
                return _cognito;
            }
        }

        public string ip = "localhost";
        public int port = 1375;

        protected override void Initialize()
        {
            ClientNetworkManager.Init();
            ClientNetworkManager.Connect(ip, port);
        }

        protected override void Dispose()
        {
            ClientNetworkManager.Stop();
        }

        // Update is called once per frame
        void Update()
        {
            ClientNetworkManager.UpdateClient();
        }
    }
}
