using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.CorporateBankAccountDTOs
{
    public class CorporateBankAccountCreateDTO
    {
        [Required]
        public string AccountType { get; set; }
        [Required]
        public string CurrencyType { get; set; }
        [Required]
        public decimal Balance { get; set; }
        [Required]
        public int CompanyOwnerId { get; set; }
        [Required]
        public int ManagerId { get; set; }
        [Required]
        public int BankId { get; set; }
    }
}
