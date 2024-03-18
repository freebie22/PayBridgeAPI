using PayBridgeAPI.Models.Currency;
using System.Globalization;

namespace PayBridgeAPI.Utility
{
    public static class Extensions
    {
        public static IEnumerable<CurrencyDTO> GetCurrency(this List<Currency> currencyResponse)
        {
            foreach (var c in currencyResponse)
            {
                CurrencyDTO currency = new CurrencyDTO()
                {
                    CurrencyCode = c.ccy,
                    NationalCurrencyCode = c.base_ccy,
                    PriceBuy = decimal.Parse(c.buy, CultureInfo.InvariantCulture),
                    PriceSell = decimal.Parse(c.sale, CultureInfo.InvariantCulture)
                };
                yield return currency;
            }
        }
    }
}
