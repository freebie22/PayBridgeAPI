namespace PayBridgeAPI.Services.RESTServices
{
    public interface ICurrencyService : IBaseService
    {
        Task<string> GetCurrencyInfo();
    }
}
