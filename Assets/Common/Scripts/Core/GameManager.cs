//-----------------------------------------------------------------
// File:         GameManager.cs
// Description:  Update entities
// Module:       Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace MonsterWorld.Core
{
    public class GameManager : IDisposable
    {
        private GameMode _activeGameMode = null;

        public GameManager() { }

        public void LoadGameMode(GameMode gameMode, Action loadCallback)
        {
            _activeGameMode = gameMode;
            _activeGameMode.Load(loadCallback);
        }

        public void OnUnityUpdate()
        {
            _activeGameMode.Update();
        }

        public void Dispose()
        {
            _activeGameMode.Dispose();
        }
    }
}