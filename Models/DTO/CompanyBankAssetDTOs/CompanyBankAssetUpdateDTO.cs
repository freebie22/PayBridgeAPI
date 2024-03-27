using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.CompanyBankAssetDTOs
{
    public class CompanyBankAssetUpdateDTO
    {
        [Required]
        public int AssetId { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
