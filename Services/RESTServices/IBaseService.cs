using PayBridgeAPI.Models;

namespace PayBridgeAPI.Services.RESTServices
{
    public interface IBaseService
    {
        Task<string> SendAsync(APIRequest request);
    }
}
