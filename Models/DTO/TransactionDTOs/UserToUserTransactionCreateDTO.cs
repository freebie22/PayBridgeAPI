namespace PayBridgeAPI.Models.DTO.TransactionDTOs
{
    public class UserToUserTransactionCreateDTO
    {
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string SenderBankCardNumber { get; set; }
        public string ReceiverBankCardNumber { get; set; }
    }
}
