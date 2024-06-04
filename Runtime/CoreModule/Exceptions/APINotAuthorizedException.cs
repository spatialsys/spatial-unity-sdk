using System;

namespace SpatialSys.UnitySDK
{
    public class APINotAuthorizedException : Exception
    {
        public APINotAuthorizedException(string message) : base(message)
        {
        }
    }
}