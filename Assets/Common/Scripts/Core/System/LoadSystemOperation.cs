//-----------------------------------------------------------------
// File:         SystemContainer.cs
// Description:  Holds system and their states
// Module:       Core
// Author:       Noé Masse
// Date:         21/03/2021
//-----------------------------------------------------------------
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MonsterWorld.Core
{
    public class LoadSystemOperation : AsyncOperationBase<GameSystem>, IUpdateReceiver
    {
        public LoadSystemOperation()
        {
        }

        public void Init(GameSystem system)
        {
            Result = system;
        }

        protected override void Execute()
        {
            Result.Initialize();
        }

        public void Update(float unscaledDeltaTime)
        {
            if (Result.IsReady)
            {
                Complete(Result, true, string.Empty);
            }
        }
    }
}