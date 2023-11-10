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

        //------------------------------------------------
        // GET USER DATA
        //------------------------------------------------

        public static IPromise<GetUserDataResponse> GetUserData()
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/sdk/me";

            IPromise<GetUserDataResponse> resp = RestClient.Get<GetUserDataResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
        }

        [Serializable]
        public struct GetUserDataResponse
        {
            public string username;
            public string email;
        }

        //------------------------------------------------
        // UPLOAD TO SANDBOX
        //------------------------------------------------

        public static IPromise<UploadSandboxBundleResponse> UploadSandboxBundle(PackageConfig packageConfig, string[] additionalBundles = null)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/sdk/v1/sandbox/bundle";

            UploadSandboxBundleRequest body = new() {
                type = PackageTypeToSAPIPackageType(packageConfig.packageType),
                sku = packageConfig.sku,
                additionalBundles = additionalBundles
            };
            if (packageConfig is SpaceConfig spaceConfig)
            {
                body.spaceInstancingEnabled = spaceConfig.settings.serverInstancingEnabled;
                body.spaceInstanceCapacity = spaceConfig.settings.serverCapacitySetting == ServerCapacitySetting.Maximum ? 0 : spaceConfig.settings.serverInstanceCapacity;
            }
            request.Body = body;

            IPromise<UploadSandboxBundleResponse> resp = RestClient.Post<UploadSandboxBundleResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
        }

        [Serializable]
        public struct UploadSandboxBundleRequest
        {
            public string type;
            public string sku;
            public string[] additionalBundles;

            // Space package type only
            public bool spaceInstancingEnabled;
            public int spaceInstanceCapacity; // 0 means platform default
        }

        [Serializable]
        public class UploadSandboxBundleResponse
        {
            public string url; // PUT bundle URL
            public string type;
            public int version;
            public ulong expiresAt;

            public string[] additionalBundleUrls; // upload URLs for additional bundles
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

            IPromise<CreateOrUpdatePackageResponse> resp = RestClient.Post<CreateOrUpdatePackageResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
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

            IPromise<UploadPackageResponse> resp = RestClient.Put<UploadPackageResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
        }

        [Serializable]
        public struct UploadPackageResponse
        {
            public string sku;
            public int version;
            public string downloadUrl;
        }

        //------------------------------------------------
        // GET PACKAGE
        //------------------------------------------------

        public static IPromise<GetPackageResponse> GetPackage(string sku)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/packages/{sku}";

            IPromise<GetPackageResponse> resp = RestClient.Get<GetPackageResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
        }

        [Serializable]
        public struct GetPackageResponse
        {
            public string name;
            public string thumbnail;
            public string sku;
            public string packageSource;
            public string packageType;
            public string creatorID;
            public string creatorName;
            public int currentVersion;
            public int latestSuccessfulVersion;

            // Only for space packages
            public string worldID;
            public string spaceID;
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

            IPromise<CreateWorldResponse> resp = RestClient.Post<CreateWorldResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
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

        public static IPromise<GetWorldsResponse> GetWorlds()
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/worlds";

            IPromise<GetWorldsResponse> resp = RestClient.Get<GetWorldsResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
        }

        [Serializable]
        public struct GetWorldsResponse
        {
            public World[] worlds;
        }

        [Serializable]
        public struct World
        {
            public string id;
            public string displayName;
        }

        //--------------------------------------------------------------------------------------------------------------
        // BADGES
        //--------------------------------------------------------------------------------------------------------------

        public static IPromise<GetBadgesResponse> GetBadges(string worldID)
        {
            RequestHelper request = CreateRequest();
            request.Uri = $"{API_ORIGIN}/v2/worlds/{worldID}/badges";

            IPromise<GetBadgesResponse> resp = RestClient.Get<GetBadgesResponse>(request);
            resp.Catch(HandleRequestException);
            return resp;
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
        }

        //------------------------------------------------
        // HELPER / PRIVATE INTERFACE
        //------------------------------------------------

        private static RequestHelper CreateRequest(int? timeout = null)
        {
            RequestHelper request = new RequestHelper();
            request.Headers["Authorization"] = $"Bearer {AuthUtility.accessToken}";
            // Example: UNITYSDK 1.2.3 official GITSHA00
            // Currently the gitsha is not used, but is included for SAPI compatibility
            request.Headers["Spatial-User-Agent"] = $"UNITYSDK {PackageManagerUtility.currentVersion} {(PackageManagerUtility.isOfficialVersion ? "official" : "dev")} 00000000";
            request.Timeout = timeout ?? 30;

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
            request.Timeout = 60 * 60; // 1 hour timeout
            if (progressCallback != null)
            {
                request.ProgressCallback += (float p) => {
                    // Unity web request progress goes from 0 to 0.5 when uploading data. Remap the upload value from 0 to 1.
                    float uploadProgress = Mathf.Clamp01(p * 2f);
                    progressCallback((long)(data.Length * uploadProgress), (long)data.Length, uploadProgress);
                };
            }
            return request;
        }

        private static void HandleRequestException(Exception exc)
        {
            if (exc is RequestException requestException && requestException.StatusCode == 401)
                AuthUtility.SetAuthStatus(AuthStatus.NotAuthenticated);
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

        //--------------------------------------------------------------------------------------------------------------
        // GENERIC MODELS
        //--------------------------------------------------------------------------------------------------------------

        public enum PackageSource
        {
            Unity
        }

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
