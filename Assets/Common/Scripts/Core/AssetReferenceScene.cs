//-----------------------------------------------------------------
// File:         AssetReferenceScene.cs
// Description:  A scene asset reference
// Module:       Unity Core
// Author:       Noé Masse
// Date:         17/03/2021
//-----------------------------------------------------------------
using System;

using UnityEngine.AddressableAssets;

namespace MonsterWorld.Unity
{
    [Serializable]
    public class AssetReferenceScene
#if UNITY_EDITOR
        : AssetReferenceT<UnityEditor.SceneAsset>
#else
        : AssetReference
#endif
    {
        public AssetReferenceScene(string guid) : base(guid) {}
    }
}