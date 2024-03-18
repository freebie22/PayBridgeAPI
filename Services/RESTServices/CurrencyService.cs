using PayBridgeAPI.Models;
using PayBridgeAPI.Utility;

namespace PayBridgeAPI.Services.RESTServices
{
    public class CurrencyService(IHttpClientFactory httpClient) : BaseService(httpClient), ICurrencyService
    {
        private IHttpClientFactory HttpClient = httpClient;
        public async Task<string> GetCurrencyInfo()
        {
            return await SendAsync(new APIRequest()
            {
                RequestType = API_Request_Type.GET,
                RequestURL = "https://api.privatbank.ua/p24api/pubinfo?exchange&json&coursid=11"
            });
        }
    }
}
