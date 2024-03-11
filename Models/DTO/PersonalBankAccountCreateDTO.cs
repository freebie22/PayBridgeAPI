using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class PersonalBankAccountCreateDTO
    {
        public string AccountType { get; set; }
        public int AccountOwnerId { get; set; }
        public int ManagerId { get; set; }
        public int BankId { get; set; }
    }
}
