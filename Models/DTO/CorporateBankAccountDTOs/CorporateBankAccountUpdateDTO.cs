using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.CorporateBankAccountDTOs
{
    public class CorporateBankAccountUpdateDTO
    {
        [Required]
        public int AccountId { get; set; }
        [Required]
        public string AccountType { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
