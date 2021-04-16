using System;

namespace AWS
{
    public interface ICognito
    {
        /// <summary>
        /// Try to sign up with email and password
        /// </summary>
        void TrySignUpRequest(string email, string password, Action<Exception> OnFailureF = null, Action OnSuccessF = null);
        /// <summary>
        /// Try to sign in with email and password
        /// </summary>
        void TrySignInRequest(string username, string password, Action<Exception> OnFailureF = null, Action<string, string> OnSuccessF = null);
        /// <summary>
        /// Try to sign in with refreshToken
        /// </summary>
        void TrySignInRequestRefreshToken(string refreshToken, Action<Exception> OnFailureF = null, Action<string> OnSuccessF = null);
        /// <summary>
        /// Take a token and try to get the user id from Cognito
        /// </summary>
        void GetUser(string token, Action<Exception> onFailure = null, Action<string> onSuccess = null);
    }
}