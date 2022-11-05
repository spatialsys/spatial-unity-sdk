using System;
using RSG;
using Proyecto26;

namespace SpatialSys.UnitySDK.Editor
{
    public static class SpatialAPI
    {
#if SPATIAL_UNITYSDK_STAGING
        public const string SPATIAL_ORIGIN = "staging.spatial.io";
#else
        public const string SPATIAL_ORIGIN = "spatial.io";
#endif

        private static readonly string API_ORIGIN = $"https://api.{SPATIAL_ORIGIN}";
        private static string _authToken => EditorUtility.GetSavedAuthToken();

        //------------------------------------------------
        // UPLOAD TEST ENVIRONMENT
        //------------------------------------------------

        public static IPromise<UploadTestEnvironmentResponse> UploadTestEnvironment()
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/sandbox/v1/test-environment";
            return RestClient.Post<UploadTestEnvironmentResponse>(request);
        }

        [Serializable]
        public struct UploadTestEnvironmentResponse
        {
            public string url;
            public int version;
            public ulong expiresAt;
        }

        //------------------------------------------------
        // UPLOAD FILE
        //------------------------------------------------

        public static IPromise<ResponseHelper> UploadFile(string url, byte[] data, Action<float> progressCallback = null)
        {
            RequestHelper request = new RequestHelper();
            request.Uri = url;
            request.BodyRaw = data;
            request.ContentType = "application/octet-stream";
            if (progressCallback != null)
                request.ProgressCallback += progressCallback;
            return RestClient.Put(request);
        }

        //------------------------------------------------
        // HELPER / PRIVATE INTERFACE
        //------------------------------------------------

        private static RequestHelper CreateRequest()
        {
            RequestHelper request = new RequestHelper();
            request.Headers["Authorization"] = $"Bearer {_authToken}";
            // Example: UNITYSDK 1.2.3 official GITSHA00
            // Currently the gitsha is not used, but is included for SAPI compatibility
            request.Headers["Spatial-User-Agent"] = $"UNITYSDK {UpgradeUtility.currentVersion} {(UpgradeUtility.isOfficialVersion ? "official" : "dev")} 00000000";
            return request;
        }
    }
}
