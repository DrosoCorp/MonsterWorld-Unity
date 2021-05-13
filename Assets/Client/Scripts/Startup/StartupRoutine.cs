using MonsterWorld.Unity.Network;
using MonsterWorld.Unity.Network.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace MonsterWorld.Unity
{
    public class StartupRoutine : MonoBehaviour
    {
        [SerializeField] private AssetReferenceScene _updaterSceneReference;
        [SerializeField] private AssetReferenceScene _loginSceneReference;
        [SerializeField] private AnimationCurve _fadeOutCurve;

        [SerializeField] private UnityEvent _loadGameMode;

        private bool _hasValidConnectionToken = false;
        private string _connectionToken;

        private Updater _updater;
        private CanvasGroup _updaterCanvasGroup;

        private AsyncOperationHandle<SceneInstance> _loadUpdaterSceneHandle;
        private AsyncOperationHandle<SceneInstance> _loadLoginSceneHandle;

        public static string localPlayerName;

        private Action<ConnectionResponsePacket> _onConnectionResponse;
        private ConnectionResponsePacket.ResponseType _connectionResponse;

        void Start()
        {
            StartCoroutine(StartupCoroutine());

            ClientNetworkManager.RegisterPacket<ConnectionPacket>();
            ClientNetworkManager.RegisterPacket<PlayerCreationPacket>();

            ClientNetworkManager.RegisterHandler<ConnectionResponsePacket>(OnConnectionResponsePacket);
            ClientNetworkManager.RegisterHandler<PlayerCreationResponsePacket>(OnPlayerCreationResponsePacket);
            ClientNetworkManager.RegisterHandler<PlayerDataPacket>(OnPlayerDataPacket);

            PlayerPrefs.DeleteAll();
        }

        #region Coroutine
        private IEnumerator StartupCoroutine()
        {
            yield return Addressables.InitializeAsync();
            yield return LoadUpdater();
            yield return RequestConnectionToken();

            if (_hasValidConnectionToken)
            {
                yield return TryAuthentication();

                switch (_connectionResponse)
                {
                    case ConnectionResponsePacket.ResponseType.Success:
                        _loadGameMode.Invoke();
                        break;
                    case ConnectionResponsePacket.ResponseType.InvalidToken:
                        yield return LoadLoginMenu();
                        break;
                    case ConnectionResponsePacket.ResponseType.RequestPlayerCreation:
                        _loadGameMode.Invoke();
                        break;
                }                
            }
            else
            {
                yield return LoadLoginMenu();
            }

            yield return FadeOutUpdater();
        }

        private IEnumerator LoadUpdater()
        {
            // Updater
            _loadUpdaterSceneHandle = _updaterSceneReference.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Additive);
            yield return _loadUpdaterSceneHandle;

            bool isUpdateTerminated = false;
            var scene = _loadUpdaterSceneHandle.Result.Scene;
            var updaterObject = scene.GetRootGameObjects()[0];

            _updater = updaterObject.GetComponent<Updater>();
            _updater.UpdateTerminated += () => isUpdateTerminated = true;
            _updaterCanvasGroup = scene.GetRootGameObjects()[1].GetComponent<CanvasGroup>();

            while (isUpdateTerminated == false) yield return null;
        }

        private IEnumerator LoadLoginMenu()
        {
            // Updater
            _loadLoginSceneHandle = _loginSceneReference.LoadSceneAsync(UnityEngine.SceneManagement.LoadSceneMode.Additive);
            yield return _loadLoginSceneHandle;

            //var scene = _loadLoginSceneHandle.Result.Scene;
            //var updaterObject = scene.GetRootGameObjects()[0];

        }

        private IEnumerator FadeOutUpdater()
        {
            float startTime = Time.time;
            float duration = _fadeOutCurve.keys[_fadeOutCurve.length - 1].time;
            float t;

            while ((t = Time.time - startTime) < duration)
            {
                _updaterCanvasGroup.alpha = _fadeOutCurve.Evaluate(t);
                yield return null;
            }

            var unloadSceneHandle = Addressables.UnloadSceneAsync(_loadUpdaterSceneHandle);
            yield return unloadSceneHandle;

            // Unity need explicit release
            _updater = null;
            _updaterCanvasGroup = null;
        }

        private IEnumerator RequestConnectionToken()
        {
            bool operationComplete = false;
            if (ClientNetworkManager.Connected)
            {
                ClientNetworkService.Instance.Authenticator.TrySignInRequestRefreshToken(
                    PlayerPrefs.GetString("RefreshToken"),
                    (_) => operationComplete = true,
                    (token) =>
                    {
                        operationComplete = true;
                        _hasValidConnectionToken = true;
                        _connectionToken = token;
                    }
                );
            }
            else
            {
                _updater.updateProgressText.Invoke("Could not connect to server...");
            }
            while (operationComplete == false) yield return null;
        }

        private IEnumerator TryAuthentication()
        {
            bool operationComplete = false;
            var connectionPacket = new ConnectionPacket()
            {
                token = _connectionToken
            };
            _onConnectionResponse = null;
            _onConnectionResponse = (packet) =>
            {
                operationComplete = true;
                _connectionResponse = packet.responseType;
            };
            ClientNetworkManager.SendPacket(connectionPacket);

            while (operationComplete == false) yield return null;
        }
        #endregion

        #region Handlers
        private void OnConnectionResponsePacket(ConnectionResponsePacket packet)
        {
            _onConnectionResponse.Invoke(packet);
        }

        private void OnPlayerCreationResponsePacket(PlayerCreationResponsePacket packet)
        {
            if (packet.responseType == PlayerCreationResponsePacket.ResponseType.Success)
            {
            }
            else
            {
            }
        }

        private void OnPlayerDataPacket(PlayerDataPacket packet)
        {
            if (packet.isLocalPlayer)
            {
                localPlayerName = packet.displayName;
            }
        }
        #endregion
    }

}
