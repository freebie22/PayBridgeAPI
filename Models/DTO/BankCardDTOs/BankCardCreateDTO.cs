using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.BankCardDTOs
{
    public class BankCardCreateDTO
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public int CVC { get; set; }
        public string CurrencyType { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public int BankAccountId { get; set; }
    }
}
