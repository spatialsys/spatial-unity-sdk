using UnityEngine;

using System;
using System.Linq;
using System.Collections.Generic;
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
                spatialSdkVersion = PackageManagerUtility.currentVersion,
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

        public static IPromise<UploadPackageResponse> UploadPackage(string sku, int version, byte[] packageFileData, Action<long, long, float> progressCallback = null)
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

        public static IPromise<ResponseHelper> UploadFile(bool useSpatialHeaders, string url, byte[] data, Action<long, long, float> progressCallback = null)
        {
            RequestHelper request = CreateUploadFileRequest(useSpatialHeaders, url, data, progressCallback);
            return RestClient.Put(request);
        }

        //------------------------------------------------
        // WORLDS
        //------------------------------------------------

        public static IPromise<CreateWorldResponse> CreateWorld()
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/worlds";
            request.Body = new CreateWorldRequest() { };

            return RestClient.Post<CreateWorldResponse>(request);
        }

        [Serializable]
        private struct CreateWorldRequest
        {
        }

        [Serializable]
        public struct CreateWorldResponse
        {
            public string id;
            public string name;
        }

        //------------------------------------------------
        // SHARED DATA TYPES/STRUCTURES
        //------------------------------------------------

        public enum PackageSource
        {
            Unity
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
            request.Headers["Spatial-User-Agent"] = $"UNITYSDK {PackageManagerUtility.currentVersion} {(PackageManagerUtility.isOfficialVersion ? "official" : "dev")} 00000000";

#if SPATIAL_UNITYSDK_STAGING
            request.EnableDebug = true;
#endif

            return request;
        }

        private static RequestHelper CreateUploadFileRequest(bool useSpatialHeaders, string url, byte[] data, Action<long, long, float> progressCallback = null)
        {
            RequestHelper request = (useSpatialHeaders) ? CreateRequest() : new RequestHelper();
            request.Uri = url;
            request.BodyRaw = data;
            request.ContentType = "application/octet-stream";
            if (progressCallback != null)
                request.ProgressCallback += p => progressCallback((long)(data.Length * p), (long)data.Length, p);
            return request;
        }

        private static string PackageSourceToSAPIPackageSource(PackageSource source)
        {
            return source.ToString();
        }

        private static string PackageTypeToSAPIPackageType(PackageType type)
        {
            if (type == PackageType.SpaceTemplate)
                return "Environment";

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

        //------------------------------------------------
        // BADGES
        //------------------------------------------------

        public static IPromise<GetBadgesResponse> GetBadges(string worldID)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/worlds/{worldID}/badges";

            return RestClient.Get<GetBadgesResponse>(request);
        }

        [Serializable]
        public struct GetBadgesResponse
        {
            public Badge[] badges;
        }

        [Serializable]
        public struct Badge
        {
            public string id;
            public string name;
            public string description;
            public string badgeIconURL;
            public string worldID;
            public string worldName;
            public string updatedAt;
            public string createdAt;
        }

        public static IPromise<CreateBadgeResponse> CreateBadge(string worldID, string name, string description)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/badges";
            request.Body = new CreateBadgeRequest() {
                name = name,
                description = description,
                worldID = worldID
            };

            return RestClient.Post<CreateBadgeResponse>(request);
        }

        [Serializable]
        public struct CreateBadgeRequest
        {
            public string name;
            public string description;
            public string worldID;
        }

        [Serializable]
        public struct CreateBadgeResponse
        {
            public string id;
            public string name;
            public string description;
            public string badgeIconURL;
            public string worldID;
            public string worldName;
            public string externalLink;
            public string updatedAt;
            public string createdAt;
        }

        public static IPromise DeleteBadge(string badgeID)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/badges/{badgeID}";

            return RestClient.Delete(request).Then(resp => { });
        }

        public static IPromise<UpdateBadgeResponse> UpdateBadge(string badgeID, string name, string description)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/badges/{badgeID}";
            request.Body = new UpdateBadgeRequest() { name = name, description = description };

            return RestClient.Put<UpdateBadgeResponse>(request);
        }

        [Serializable]
        public struct UpdateBadgeRequest
        {
            public string name;
            public string description;
        }

        [Serializable]
        public struct UpdateBadgeResponse
        {
            public string id;
            public string name;
            public string description;
            public string badgeIconURL;
            public string worldID;
            public string worldName;
            public string externalLink;
            public string updatedAt;
            public string createdAt;
        }

        public static IPromise<UploadBadgeIconResponse> UploadBadgeIcon(string badgeID, byte[] data)
        {
            string url = $"{API_ORIGIN}/v2/badges/{badgeID}/icon";
            RequestHelper request = CreateUploadFileRequest(useSpatialHeaders: true, url, data, null);
            return RestClient.Put<UploadBadgeIconResponse>(request);
        }

        [Serializable]
        public struct UploadBadgeIconResponse
        {
            public string id;
            public string name;
            public string description;
            public string badgeIconURL;
            public string worldID;
            public string worldName;
            public string externalLink;
            public string updatedAt;
            public string createdAt;
        }
    }
}
