namespace PayBridgeAPI.Models.DTO.TransactionDTOs
{
    public class CompanyToCompanyTransactionCreateDTO
    {
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string TransactionType { get; set; }
        public string SenderIBANNumber { get; set; }
        public string ReceiverIBANNumber { get; set; }
    }
}
