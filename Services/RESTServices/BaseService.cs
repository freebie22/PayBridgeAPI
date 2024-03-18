using Newtonsoft.Json;
using PayBridgeAPI.Models;
using PayBridgeAPI.Utility;
using System.Text;
using System.Text.Json;

namespace PayBridgeAPI.Services.RESTServices
{
    public class BaseService : IBaseService
    {
        public IHttpClientFactory _httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendAsync(APIRequest request)
        {
            var client = _httpClient.CreateClient("PayBridgeCurrencyAPI");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(request.RequestURL);
            if(request.Data != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), encoding: Encoding.UTF8, "application/json");
            }
            switch(request.RequestType)
            {
                case API_Request_Type.POST:
                    message.Method = HttpMethod.Post;
                    break;
                case API_Request_Type.PUT:
                    message.Method = HttpMethod.Put;
                    break;
                case API_Request_Type.DELETE:
                    message.Method = HttpMethod.Delete;
                    break;
                default: 
                    message.Method = HttpMethod.Get;
                    break;
            }

            HttpResponseMessage response = await client.SendAsync(message);
            var apiContent = await response.Content.ReadAsStringAsync();
            return apiContent;
        }
    }
}
