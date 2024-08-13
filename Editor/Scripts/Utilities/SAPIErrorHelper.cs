using System;
using System.Linq;

namespace SpatialSys.UnitySDK.Editor
{
    public enum SAPIErrorCode
    {
        UNKNOWN = 0,

        NOT_OWNER_OF_PACKAGE,
        PACKAGE_UPLOAD_LIMIT_REACHED,
        USER_PROFILE_NOT_FOUND,
    }

    public static class SAPIErrorHelper
    {
        /// <summary>
        /// Converts string to a SAPIErrorCode type. Returns UNKNOWN if unable to parse.
        /// </summary>
        public static SAPIErrorCode ParseErrorCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return SAPIErrorCode.UNKNOWN;

            // The codes should generally be all-caps, but let's ignore casing to be extra safe.
            return Enum.TryParse<SAPIErrorCode>(code, ignoreCase: true, out SAPIErrorCode parsedCode) ? parsedCode : SAPIErrorCode.UNKNOWN;
        }

        /// <summary>
        /// Parses an exception as a SAPI error type.
        /// Returns false if it failed to parse, or if the exception is not a SAPI error.
        /// </summary>
        public static bool TryParse(Exception ex, out SAPIErrorResponse sapiError)
        {
            sapiError = null;
            if (!(ex is Proyecto26.RequestException reqException))
                return false;

            try
            {
                sapiError = UnityEngine.JsonUtility.FromJson<SAPIErrorResponse>(reqException.Response);
            }
            catch { }

            // Always ensure there's always one error entry
            // Check validity
            if (sapiError == null || sapiError.errors == null || sapiError.errors.Length == 0)
            {
                sapiError = new SAPIErrorResponse();
                sapiError.errors = new SAPIErrorResponse.Error[1];
                sapiError.errors[0].code = "UNKNOWN";
                sapiError.errors[0].message = "Unknown error";
                sapiError.errors[0].statusCode = 0;
                return false;
            }

            return true;
        }
    }

    [Serializable]
    public class SAPIErrorResponse
    {
        public Error[] errors;

        public string firstErrorCodeRaw => errors != null && errors.Length > 0 ? errors[0].code : null;
        public SAPIErrorCode firstErrorCode => SAPIErrorHelper.ParseErrorCode(firstErrorCodeRaw);

        public bool HasErrorCode(SAPIErrorCode targetCode)
        {
            return errors != null && errors.Length > 0 && errors.Any(e => SAPIErrorHelper.ParseErrorCode(e.code) == targetCode);
        }

        [Serializable]
        public struct Error
        {
            public string code;
            public string message;
            public int statusCode;
        }
    }
}
