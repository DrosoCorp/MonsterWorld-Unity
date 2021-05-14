//-----------------------------------------------------------------
// File:         CharacterProviderSystem.cs
// Description:  Store and provide characters data
// Module:       Client Systems
// Author:       Noé Masse
// Date:         16/04/2021
//-----------------------------------------------------------------
using System;
using UnityEngine;

using MonsterWorld.Unity.Network;
using MonsterWorld.Unity.Network.Client;
using System.Collections;

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

                ClientNetworkManager.OnPlayerDataPacket += OnPlayerData;
                var requestPlayerDataPacket = new RequestPlayerDataPacket();
                ClientNetworkManager.SendPacket(ref requestPlayerDataPacket);
            };
        }

        private void OnPlayerData(ref PlayerDataPacket packet)
        {
            ClientNetworkManager.OnPlayerDataPacket -= OnPlayerData;
            LocalPlayer = StoreCharacter(packet.displayName);
            TerminateInitialization();
        }

        public Character StoreCharacter(string displayName)
        {
            var go = Instantiate(characterPrefab, _characterPool.transform);
            go.name = $"Player ({displayName})";
            return go.GetComponent<Character>();
            //chara.Sprite = ...
        }

        public override void Dispose()
        {
            UnloadScenes(scene);
        }
    }
}
