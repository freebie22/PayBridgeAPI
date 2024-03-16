using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.BankCardDTOs
{
    public class BankCardDTO
    {
        public int BankCardId { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public int CVC { get; set; }
        public string OwnerCredentials { get; set; }
        public string CurrencyType { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public string RegistrationDate { get; set; }
    }
}
