using PayBridgeAPI.Utility;

namespace PayBridgeAPI.Models
{
    public sealed class APIRequest
    {
        public API_Request_Type RequestType { get; set; }
        public string RequestURL { get; set; }
        public object Data { get; set; }
    }
}
