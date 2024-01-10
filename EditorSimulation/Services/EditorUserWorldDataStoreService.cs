using SpatialSys.UnitySDK;

namespace SpatialSys.UnitySDK.EditorSimulation
{
    public class EditorUserWorldDataStoreService : IUserWorldDataStoreService
    {
        public DataStoreGetVariableRequest GetVariable(string key, object defaultValue)
        {
            DataStoreGetVariableRequest request = new() {
                responseCode = DataStoreResponseCode.UnknownError,
                value = defaultValue,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreOperationRequest SetVariable(string key, object value)
        {
            DataStoreOperationRequest request = new() {
                responseCode = DataStoreResponseCode.UnknownError,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreOperationRequest DeleteVariable(string key)
        {
            DataStoreOperationRequest request = new() {
                responseCode = DataStoreResponseCode.UnknownError,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreHasVariableRequest HasVariable(string key)
        {
            DataStoreHasVariableRequest request = new() {
                responseCode = DataStoreResponseCode.UnknownError,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreHasAnyVariableRequest HasAnyVariable()
        {
            DataStoreHasAnyVariableRequest request = new() {
                responseCode = DataStoreResponseCode.UnknownError,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreOperationRequest ClearAllVariables()
        {
            DataStoreOperationRequest request = new() {
                responseCode = DataStoreResponseCode.UnknownError,
            };
            request.InvokeCompletionEvent();
            return request;
        }

        public DataStoreDumpVariablesRequest DumpVariablesAsJSON()
        {
            DataStoreDumpVariablesRequest request = new() {
                responseCode = DataStoreResponseCode.UnknownError,
            };
            request.InvokeCompletionEvent();
            return request;
        }
    }
}
