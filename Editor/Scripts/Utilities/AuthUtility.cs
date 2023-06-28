using RSG;
using System;
using System.Text.RegularExpressions;

using UnityEditor;
using UnityEngine;

namespace SpatialSys.UnitySDK.Editor
{
    public enum AuthStatus
    {
        NotAuthenticated = 0,
        Authenticating = 1,
        FailedToAuthenticate = 2,
        Authenticated = 3,
    }

    public static class AuthUtility
    {
        private const string ACCESS_TOKEN_PREFS_KEY = "SpatialSDK_Token";
        private const string AUTH_STATUS_SESSION_PREFS_KEY = "SpatialSDK_AuthStatus";
        private const string FIRST_INIT_SESSION_PREFS_KEY = "SpatialSDK_FirstInit";
        private const string USERNAME_SESSION_PREFS_KEY = "SpatialSDK_AuthUtility_Username";
        private const string EMAIL_SESSION_PREFS_KEY = "SpatialSDK_AuthUtility_Email";

        public static string accessToken => EditorPrefs.GetString(ACCESS_TOKEN_PREFS_KEY);
        public static bool isAuthenticating => authStatus == AuthStatus.Authenticating;
        public static bool isAuthenticated => authStatus == AuthStatus.Authenticated;
        public static string userAlias
        {
            get
            {
                string username = SessionState.GetString(USERNAME_SESSION_PREFS_KEY, null);
                string email = SessionState.GetString(EMAIL_SESSION_PREFS_KEY, null);

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email))
                    return null;

                return $"{username} ({email})";
            }
        }

        public static AuthStatus authStatus { get; private set; }

        public static Action onAuthStatusChanged;

        static AuthUtility()
        {
            // Restore field values after recompile
            SetAuthStatus((AuthStatus)SessionState.GetInt(AUTH_STATUS_SESSION_PREFS_KEY, defaultValue: (int)AuthStatus.NotAuthenticated));

#if !SPATIAL_UNITYSDK_INTERNAL
            bool isFirstInit = false;
            if (!SessionState.GetBool(FIRST_INIT_SESSION_PREFS_KEY, defaultValue: false))
            {
                SessionState.SetBool(FIRST_INIT_SESSION_PREFS_KEY, true);
                isFirstInit = true;
            }

            // Only show failure dialog on the first initialization (usually when opening the editor).
            // Subsequent failures will be handled when calling other API endpoints like uploading to sandbox, uploading package, etc.
            ReauthenticateSessionIfNecessary(showFailureDialog: isFirstInit);
#endif
        }

        private static void ReauthenticateSessionIfNecessary(bool showFailureDialog)
        {
            if (!string.IsNullOrWhiteSpace(accessToken) && !isAuthenticated)
            {
                LogInWithDialogPopup(accessToken, showFailureDialog);
            }
        }

        public static IPromise LogIn(string token)
        {
            if (string.IsNullOrWhiteSpace(token) || !IsValidAccessTokenFormat(token))
                return Promise.Rejected(new FormatException("Failed to login since the access token is empty or incorrectly formatted"));

            if (accessToken == token && isAuthenticated)
                return Promise.Resolved();

            SetAuthStatus(AuthStatus.Authenticating);
            EditorPrefs.SetString(ACCESS_TOKEN_PREFS_KEY, token);

            return SpatialAPI.GetUserData()
                .Then(resp => {
                    SetUserSessionInfo(resp.username, resp.email);
                    SetAuthStatus(AuthStatus.Authenticated);
                })
                .Catch(exc => {
                    LogOut();
                    Debug.LogException(exc);
                });
        }

        public static IPromise LogInWithDialogPopup(string token, bool showFailureDialog = true)
        {
            return LogIn(token)
                .Catch(exc => {
                    if (showFailureDialog)
                        UnityEditor.EditorUtility.DisplayDialog("Failed to log in", exc.Message, "OK");
                });
        }

        public static void LogOut()
        {
            EditorPrefs.DeleteKey(ACCESS_TOKEN_PREFS_KEY);
            ClearUserSessionInfo();
            SetAuthStatus(AuthStatus.NotAuthenticated);
        }

        public static bool IsValidAccessTokenFormat(string token)
        {
            return Regex.IsMatch(token, "^[a-zA-Z0-9._-]+$");
        }

        private static void SetAuthStatus(AuthStatus status)
        {
            authStatus = status;
            SessionState.SetInt(AUTH_STATUS_SESSION_PREFS_KEY, (int)status);
            onAuthStatusChanged?.Invoke();
        }

        private static void SetUserSessionInfo(string username, string email)
        {
            SessionState.SetString(USERNAME_SESSION_PREFS_KEY, username);
            SessionState.SetString(EMAIL_SESSION_PREFS_KEY, email);
        }

        private static void ClearUserSessionInfo()
        {
            SessionState.EraseString(USERNAME_SESSION_PREFS_KEY);
            SessionState.EraseString(EMAIL_SESSION_PREFS_KEY);
        }
    }
}
