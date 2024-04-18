using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class PersonalBankAccountUpdateDTO
    {
        public string AccountType { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public ICollection<BankCard> BankCards { get; set; } = new List<BankCard>();
    }
}
