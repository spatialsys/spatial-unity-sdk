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
            request.Uri = $"{API_ORIGIN}/unity/test-environment";
            return RestClient.Post<UploadTestEnvironmentResponse>(request);
        }

        [Serializable]
        public struct UploadTestEnvironmentResponse
        {
            public string uploadUrl;
            public int version;
            public ulong expiresAt;
        }

        //------------------------------------------------
        // UPLOAD FILE
        //------------------------------------------------

        public static IPromise<ResponseHelper> UploadFile(string url, byte[] data)
        {
            RequestHelper request = new RequestHelper();
            request.Uri = url;
            request.BodyRaw = data;
            request.ContentType = "application/octet-stream";
            return RestClient.Put(request);
        }

        //------------------------------------------------
        // HELPER / PRIVATE INTERFACE
        //------------------------------------------------

        private static RequestHelper CreateRequest()
        {
            RequestHelper request = new RequestHelper();
            request.Headers["Authorization"] = $"Bearer {_authToken}";
            return request;
        }
    }
}
