using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.CompanyBankAssetDTOs
{
    public class CompanyBankAssetDTO
    {
        public int AssetId { get; set; }
        public string AssetUniqueNumber { get; set; }
        public string IBAN_Number { get; set; }
        public string CurrencyType { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankAccountUniqueNumber { get; set; }
        public string ShortCompanyOwnerName { get; set; }
        public string RegistrationDate { get; set; }
    }
}
