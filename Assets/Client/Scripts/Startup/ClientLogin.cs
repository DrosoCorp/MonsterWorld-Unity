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

using MonsterWorld.Unity.Network.Client;

namespace MonsterWorld.Unity
{
    public class ClientLogin : MonoBehaviour
    {
        [SerializeField] private TMP_InputField emailField;
        [SerializeField] private TMP_InputField passwordField;

        [SerializeField] private TextMeshProUGUI errorText;

        public Action<string> OnConnectionToken;

        public void OnButtonLogin()
        {
            ClientNetworkService.Instance.Authenticator.TrySignInRequest(emailField.text, passwordField.text, ShowException, OnLoginSuccess);
        }

        public void OnButtonSignup()
        {
            ClientNetworkService.Instance.Authenticator.TrySignUpRequest(emailField.text, passwordField.text, ShowException, ShowSignupSuccess);
        }

        private void OnLoginSuccess(string connectionToken, string refreshToken)
        {
            PlayerPrefs.SetString("RefreshToken", refreshToken);
            OnConnectionToken(connectionToken);
        }

        private void ShowException(Exception exception)
        {
            errorText.text = exception.Message;
        }

        private void ShowSignupSuccess()
        {
            errorText.text = "SignUp Success !";
        }
    }

}
