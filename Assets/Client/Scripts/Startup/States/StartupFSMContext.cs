using UnityEngine;

namespace MonsterWorld.Unity.Startup
{
    public static class StartupFSMContext
    {
        public static class Parameters
        {
            public static int StartupCompleteId = Animator.StringToHash("StartupComplete");
            public static int UpdateCompleteId = Animator.StringToHash("UpdateComplete");
            public static int AutoAuthenticationSuccessId = Animator.StringToHash("AutoAuthenticationSuccess");
            public static int AutoAuthenticationFailureId = Animator.StringToHash("AutoAuthenticationFailure");
            public static int HasConnectionTokenId = Animator.StringToHash("HasConnectionToken");
            public static int ConnectionToServerFailedId = Animator.StringToHash("ConnectionToServerFailed");
            public static int RequestPlayerDataId = Animator.StringToHash("RequestPlayerData");
            public static int PlayerConnectedId = Animator.StringToHash("PlayerConnected");

        }

        public static string connectionToken;
    }
}