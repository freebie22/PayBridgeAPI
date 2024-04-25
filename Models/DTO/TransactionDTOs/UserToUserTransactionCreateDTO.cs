namespace PayBridgeAPI.Models.DTO.TransactionDTOs
{
    public class UserToUserTransactionCreateDTO
    {
        public decimal Amount { get; set; }
        public string SenderBankCardNumber { get; set; }
        public string ReceiverBankCardNumber { get; set; }
        public string RechargingType { get; set; } = "";
        public string SenderExpiryDate { get; set; } = "";
        public int? SenderCVC { get; set; } = null;
    }
}
