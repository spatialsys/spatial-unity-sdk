using UnityEngine;

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
        // UPLOAD TO SANDBOX
        //------------------------------------------------

        public static IPromise<UploadSandboxBundleResponse> UploadSandboxBundle(PackageType packageType)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/sdk/v1/sandbox/bundle";
            request.Body = new UploadSandboxBundleRequest() {
                type = PackageTypeToSAPIPackageType(packageType)
            };
            return RestClient.Post<UploadSandboxBundleResponse>(request);
        }

        [Serializable]
        public struct UploadSandboxBundleRequest
        {
            public string type;
        }

        [Serializable]
        public struct UploadSandboxBundleResponse
        {
            public string url; // PUT bundle URL
            public string type;
            public int version;
            public ulong expiresAt;
        }

        //------------------------------------------------
        // CREATE PACKAGE
        //------------------------------------------------

        public static IPromise<CreateOrUpdatePackageResponse> CreateOrUpdatePackage(string sku, PackageType packageType)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/sdk/v1/package/";
            request.Body = new CreateOrUpdatePackageRequest() {
                sku = sku,
                unityVersion = Application.unityVersion,
                spatialSdkVersion = UpgradeUtility.currentVersion,
                packageSource = PackageSourceToSAPIPackageSource(PackageSource.Unity),
                packageType = PackageTypeToSAPIPackageType(packageType)
            };

            return RestClient.Post<CreateOrUpdatePackageResponse>(request);
        }

        [Serializable]
        private struct CreateOrUpdatePackageRequest
        {
            public string sku;
            public string unityVersion;
            public string spatialSdkVersion;
            public string packageSource;
            public string packageType;
        }

        [Serializable]
        public struct CreateOrUpdatePackageResponse
        {
            public string sku;
            public int version;
        }

        //------------------------------------------------
        // UPLOAD PACKAGE
        //------------------------------------------------

        public static IPromise<UploadPackageResponse> UploadPackage(string sku, int version, byte[] packageFileData, Action<float> progressCallback = null)
        {
            string url = $"{API_ORIGIN}/sdk/v1/package/{sku}/{version}";
            RequestHelper request = CreateUploadFileRequest(useSpatialHeaders: true, url, packageFileData, progressCallback);
            return RestClient.Put<UploadPackageResponse>(request);
        }

        [Serializable]
        public struct UploadPackageResponse
        {
            public string sku;
            public int version;
            public string downloadUrl;
        }

        //------------------------------------------------
        // UPLOAD FILE
        //------------------------------------------------

        public static IPromise<ResponseHelper> UploadFile(bool useSpatialHeaders, string url, byte[] data, Action<float> progressCallback = null)
        {
            RequestHelper request = CreateUploadFileRequest(useSpatialHeaders, url, data, progressCallback);
            return RestClient.Put(request);
        }

        //------------------------------------------------
        // SHARED DATA TYPES/STRUCTURES
        //------------------------------------------------

        public enum PackageSource
        {
            Unity
        }

        // NOTE: keep this the same as UnitySDK PackageType enum
        public enum PackageType
        {
            Environment = 0,
            Avatar = 1,
            AvatarAnimation = 2,
            PrefabObject = 3,
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

        private static RequestHelper CreateUploadFileRequest(bool useSpatialHeaders, string url, byte[] data, Action<float> progressCallback = null)
        {
            RequestHelper request = (useSpatialHeaders) ? CreateRequest() : new RequestHelper();
            request.Uri = url;
            request.BodyRaw = data;
            request.ContentType = "application/octet-stream";
            if (progressCallback != null)
                request.ProgressCallback += progressCallback;
            return request;
        }

        private static string PackageSourceToSAPIPackageSource(PackageSource source)
        {
            return source.ToString();
        }

        private static string PackageTypeToSAPIPackageType(PackageType type)
        {
            return type.ToString();
        }

        public static bool TryGetSingleError(string response, out ErrorResponse.Error error)
        {
            try
            {
                ErrorResponse errResp = JsonUtility.FromJson<ErrorResponse>(response);
                if (errResp.errors.Length > 0)
                {
                    error = errResp.errors[0];
                    return true;
                }
            }
            catch { }

            error = new ErrorResponse.Error();
            return false;
        }

        //------------------------------------------------
        // Models
        //------------------------------------------------

        [Serializable]
        public struct ErrorResponse
        {
            [Serializable]
            public struct Error
            {
                public string code;
                public string message;
                public int statusCode;
                public bool display;
                public ErrorField[] fields;
            }

            [Serializable]
            public struct ErrorField
            {
                public string field;
                public string error;
            }

            public Error[] errors;
            public string trace;
        }
    }
}
