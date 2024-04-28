using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.CorporateBankAccountDTOs
{
    public class CorporateBankAccountCreateDTO
    {
        [Required]
        public int CompanyOwnerId { get; set; }
        [Required]
        public int BankId { get; set; }
    }
}
