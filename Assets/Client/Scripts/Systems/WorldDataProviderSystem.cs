//-----------------------------------------------------------------
// File:         WorldDataProviderSystem.cs
// Description:  Store and provide world data (tilemaps, navigation graph etc...)
// Module:       Client Systems
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System;

using UnityEngine;
using UnityEngine.AddressableAssets;
using MonsterWorld.Unity.Navigation;

namespace MonsterWorld.Unity.Systems
{
    [DisallowMultipleComponent]
    public class WorldDataProviderSystem : GameSystem
    {
        public AssetReferenceScene scene = null;

        private GameObject _worldDataObject = null;
        private NavGraphContainer _navGraphContainer = null;

        public ClientNavigationGraph NavGraph => _navGraphContainer.Graph;

        public override void Initialize()
        {
            LoadScenes(scene).Completed += (result) =>
            {
                _worldDataObject = GameObject.Find("WorldData");
                _navGraphContainer = _worldDataObject.GetComponentInChildren<NavGraphContainer>();
                TerminateInitialization();
            };
        }

        public override void Dispose()
        {
            UnloadScenes(scene);
        }
    }
}
