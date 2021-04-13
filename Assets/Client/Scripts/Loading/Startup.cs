using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

namespace MonsterWorld.Unity
{
    public class Startup : MonoBehaviour
    {
        public string assetKey = "";
        public TextMeshProUGUI updateTextGUI = null;

        void Start()
        {
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            Addressables.InitializeAsync().Completed += OnAddressablesInitialized;
        }

        private void OnAddressablesInitialized(AsyncOperationHandle<IResourceLocator> obj)
        {
            Addressables.CheckForCatalogUpdates().Completed += UpdateCatalogs;
        }

        private void UpdateCatalogs(AsyncOperationHandle<List<string>> catalogs)
        {
            if (catalogs.Result.Count > 0)
            {
                Addressables.UpdateCatalogs().Completed += (updates) => StartCoroutine(DownloadAssets());
            }
            else
            {
                StartCoroutine(DownloadAssets());
            }
        }

        private IEnumerator DownloadAssets()
        {
            Addressables.ClearDependencyCacheAsync(assetKey);

            yield return new WaitForSeconds(5f);

            var handle = Addressables.DownloadDependenciesAsync(assetKey);
            while (handle.IsDone == false)
            {
                Debug.Log(handle.PercentComplete);
                updateTextGUI.text = "Downloading Asset... " + assetKey + " " + handle.PercentComplete * 100f + "%";
                yield return null;
            }
            Debug.Log(handle.Result);
        }
    }

}
