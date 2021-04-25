//-----------------------------------------------------------------
// File:         CameraSystem.cs
// Description:  Manage and move the cameras
// Module:       Client Systems
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using UnityEngine;
using MonsterWorld.Unity.Systems.Characters;

namespace MonsterWorld.Unity.Systems
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterStoreSystem))]
    public class CameraSystem : GameSystem
    {
        private CharacterStoreSystem _CharacterStoreSystem;

        public override void Initialize()
        {
            _CharacterStoreSystem = GetSystem<CharacterStoreSystem>();
            Camera.main.transform.SetParent(_CharacterStoreSystem.LocalPlayer.transform, false);
            TerminateInitialization();
        }

        public override void Dispose()
        {
        }
    }
}
