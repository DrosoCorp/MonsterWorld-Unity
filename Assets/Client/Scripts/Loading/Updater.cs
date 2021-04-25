using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MonsterWorld.Unity
{
    public class Updater : MonoBehaviour
    {
        public AssetLabelReference assetLabel;
        public UnityEvent<string> updateProgressText;
        public UnityEvent<float> updateProgress;
        public UnityEvent loadGameMode;

        void Start()
        {
            StartCoroutine(StartupRoutine());
        }

        private IEnumerator StartupRoutine()
        {
            // Initialize
            updateProgressText.Invoke("Checking for Updates...");
            yield return Addressables.InitializeAsync();

            // Update Catalogs
            var checkForUpdateHandle = Addressables.CheckForCatalogUpdates(autoReleaseHandle: false);
            yield return checkForUpdateHandle;
            var catalogs = checkForUpdateHandle.Result;
            Addressables.Release(checkForUpdateHandle);

            if (catalogs.Count > 0)
            {
                yield return Addressables.UpdateCatalogs();
            }

            // Download Updates
            var downloadAssetsHandle = Addressables.DownloadDependenciesAsync(assetLabel);
            while (downloadAssetsHandle.IsDone == false)
            {
                updateProgress.Invoke(downloadAssetsHandle.PercentComplete);
                updateProgressText.Invoke($"Updating ... {downloadAssetsHandle.PercentComplete * 100f:F0} %");
                yield return null;
            }
            updateProgress.Invoke(1.0f);
            Addressables.Release(downloadAssetsHandle);

            updateProgressText.Invoke("Connecting...");
            yield return new WaitForSeconds(2f);

            updateProgressText.Invoke("Loading...");
            loadGameMode.Invoke();
        }
    }

}
