using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

using Amazon;
using Amazon.Runtime;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using System.Linq.Expressions;

namespace AWS
{
    public class Cognito : ICognito
    {
        // All these settings are found in the User Pool page on AWS Management Console
        public const string AppClientID = "22qjiqao6au9p7d079o8r1fqje";       // find this under "App Client Settings"
        public const string UserPoolId = "us-east-2_0IxDIAL55";               // Pool Id on the General Settings page
        public const string UserPoolName = "0IxDIAL55";                       // the bit at the end of UserPoolID, after the region
        RegionEndpoint CognitoIdentityRegion = RegionEndpoint.USEast2;


        /// <summary>
        /// Cognito IDP Client is constructed on-demand
        /// </summary>
        private AmazonCognitoIdentityProviderClient CognitoIDPClient
        {
            get
            {
                if (_cgClient == null)
                {
                    var config = new AmazonCognitoIdentityProviderConfig();
                    config.RegionEndpoint = CognitoIdentityRegion;
                    _cgClient = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), config);
                }
                return _cgClient;
            }
        }
        private AmazonCognitoIdentityProviderClient _cgClient = null;




        /// <summary>
        /// Try to sign up with given email & password.
        /// </summary>
        public async void TrySignUpRequest(string email, string password,
            Action<Exception> OnFailureF = null, Action OnSuccessF = null)
        {
            SignUpRequest signUpRequest = new SignUpRequest()
            {
                ClientId = AppClientID,
                Password = password,
                Username = email,
            };
            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = email
            };
            signUpRequest.UserAttributes.Add(emailAttribute);

            try
            {
                var response = await CognitoIDPClient.SignUpAsync(signUpRequest);
                if (OnSuccessF != null)
                    OnSuccessF();
            }
            catch (Exception e)
            {
                if (OnFailureF != null)
                    OnFailureF(e);
            }
        }

        /// <summary>
        /// Try to sign in with email and password
        /// </summary>
        public async void TrySignInRequest(string username, string password,
            Action<Exception> OnFailureF = null, Action<string> OnSuccessF = null)
        {
            //Get the SRP variables A and a
            var TupleAa = AuthenticationHelper.CreateAaTuple();

            InitiateAuthRequest authRequest = new InitiateAuthRequest()
            {
                ClientId = AppClientID,
                AuthFlow = AuthFlowType.USER_SRP_AUTH,
                AuthParameters = new Dictionary<string, string>() {
                    { "USERNAME", username },
                    { "SRP_A", TupleAa.Item1.ToString(16) } }
            };

            //
            // This is a nested request / response / request. First we send the
            // InitiateAuthRequest, with some crypto things. AWS sends back
            // some of its own crypto things, in the authResponse object (this is the "challenge").
            // We combine that with the actual password, using math, and send it back (the "challenge response").
            // If AWS is happy with our answer, then it is convinced we know the password,
            // and it sends us some tokens!
            try {
                var authResponse = await CognitoIDPClient.InitiateAuthAsync(authRequest);

                //The timestamp format returned to AWS _needs_ to be in US Culture
                DateTime timestamp = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
                CultureInfo usCulture = new CultureInfo("en-US");
                String timeStr = timestamp.ToString("ddd MMM d HH:mm:ss \"UTC\" yyyy", usCulture);


                //Do the hard work to generate the claim we return to AWS
                var challegeParams = authResponse.ChallengeParameters;
                byte[] claim = AuthenticationHelper.authenticateUser(
                                    challegeParams["USERNAME"],
                                    password, UserPoolName, TupleAa,
                                    challegeParams["SALT"], challegeParams["SRP_B"],
                                    challegeParams["SECRET_BLOCK"], timeStr);

                String claimBase64 = System.Convert.ToBase64String(claim);

                // construct the second request
                RespondToAuthChallengeRequest respondRequest = new RespondToAuthChallengeRequest()
                {
                    ChallengeName = authResponse.ChallengeName,
                    ClientId = AppClientID,
                    ChallengeResponses = new Dictionary<string, string>() {
                        { "PASSWORD_CLAIM_SECRET_BLOCK", challegeParams["SECRET_BLOCK"] },
                        { "PASSWORD_CLAIM_SIGNATURE", claimBase64 },
                        { "USERNAME", username },
                        { "TIMESTAMP", timeStr } }
                };

                // send the second request
                try
                {
                    var finalResponse = await CognitoIDPClient.RespondToAuthChallengeAsync(respondRequest);

                    // Ok, if we got here, we logged in, and here are some tokens
                    AuthenticationResultType authResult = finalResponse.AuthenticationResult;
                    string idToken = authResult.IdToken;
                    string accessToken = authResult.AccessToken;
                    string refreshToken = authResult.RefreshToken;

                    if (OnSuccessF != null)
                        OnSuccessF(accessToken);
                }
                catch (Exception e)
                {
                    if (OnFailureF != null)
                        OnFailureF(e);
                    return;
                }
            }
            catch (Exception e)
            {
                if (OnFailureF != null)
                    OnFailureF(e);
                return;
            }
        }

        public async void GetUser(string token, Action<Exception> onFailure = null, Action<string> onSuccess = null)
        {
            GetUserRequest request = new GetUserRequest()
            {
                AccessToken = token
            };

            try
            {
                var response = await CognitoIDPClient.GetUserAsync(request);
                if (onSuccess != null)
                {
                    onSuccess(response.Username);
                }
            }
            catch (Exception e)
            {
                if (onFailure != null)
                {
                    onFailure(e);
                }
            }
        }
    }
}
