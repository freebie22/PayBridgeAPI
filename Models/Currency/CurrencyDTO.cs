namespace PayBridgeAPI.Models.Currency
{
    public class CurrencyDTO
    {
        public string CurrencyCode { get; set; }
        public string NationalCurrencyCode { get; set; }
        public decimal PriceBuy { get; set; }
        public decimal PriceSell { get; set;}
    }
}
