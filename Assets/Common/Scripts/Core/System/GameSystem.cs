//-----------------------------------------------------------------
// File:         GameSystem.cs
// Description:  Update entities
// Module:       Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System;

namespace MonsterWorld.Core
{
    public abstract class GameSystem : IDisposable
    {
        private bool _isReady;
        public bool IsReady => _isReady;

        public abstract void Initialize();

        public abstract void Update();

        public virtual void Dispose() {}

        public void TerminateInitialization()
        {
            _isReady = true;
        }
    }
}
