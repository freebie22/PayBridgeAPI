using System.Net;

namespace PayBridgeAPI.Models
{
    public class APIResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public object Result { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
        public bool IsSuccess { get; set; }
    }
}
