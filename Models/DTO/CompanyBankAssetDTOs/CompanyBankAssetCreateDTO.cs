using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.CompanyBankAssetDTOs
{
    public class CompanyBankAssetCreateDTO
    {
        [Required]
        public string IBAN_Number { get; set; }
        [Required]
        public int CorporateAccountId { get; set; }
    }
}
