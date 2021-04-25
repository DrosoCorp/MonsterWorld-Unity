//-----------------------------------------------------------------
// File:         CharacterProviderSystem.cs
// Description:  Store and provide characters data
// Module:       Client Systems
// Author:       Noé Masse
// Date:         16/04/2021
//-----------------------------------------------------------------
using UnityEngine;

namespace MonsterWorld.Unity.Systems.Characters
{
    [DisallowMultipleComponent]
    public class CharacterStoreSystem : GameSystem
    {
        public AssetReferenceScene scene = null;
        public GameObject characterPrefab = null;

        private GameObject _characterPool = null;

        public Character LocalPlayer { get; private set; } = null;

        public override void Initialize()
        {
            LoadScenes(scene).Completed += (result) =>
            {
                _characterPool = GameObject.Find("CharacterPool");
                StoreCharacter();
                TerminateInitialization();
            };
        }

        public void StoreCharacter()
        {
            var go = Instantiate(characterPrefab, _characterPool.transform);
            LocalPlayer = go.GetComponent<Character>();
            //chara.Sprite = ...
        }

        public override void Dispose()
        {
            UnloadScenes(scene);
        }
    }
}
