namespace PayBridgeAPI.Models.DTO.TransactionDTOs
{
    public class CompanyToUserTransactionCreateDTO
    {
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string SenderIBANNumber { get; set; }
        public string ReceiverBankCardNumber { get; set; }
    }
}
