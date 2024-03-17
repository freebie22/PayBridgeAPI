using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PayBridgeAPI.Models.DTO.BankCardDTOs;

namespace PayBridgeAPI.Models.DTO
{
    public class PersonalBankAccountDTO
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountOwnerFullName { get; set; }
        public string AccountType { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string RegistratedByManager { get; set; }
        public string BankName { get; set; }
        public ICollection<BankCardDTO> BankCards { get; set; }
        public string RegistrationDate { get; set; }
    }
}
