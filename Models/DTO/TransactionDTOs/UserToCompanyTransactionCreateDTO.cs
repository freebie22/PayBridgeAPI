namespace PayBridgeAPI.Models.DTO.TransactionDTOs
{
    public class UserToCompanyTransactionCreateDTO
    {
        public decimal Amount { get; set; } 
        public string Description { get; set; }
        public string SenderBankCardNumber { get; set; }
        public string ReceiverIBANNumber { get; set; }
    }
}
