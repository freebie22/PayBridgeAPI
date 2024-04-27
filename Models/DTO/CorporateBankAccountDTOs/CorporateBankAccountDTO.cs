using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PayBridgeAPI.Models.DTO.CompanyBankAssetDTOs;

namespace PayBridgeAPI.Models.DTO.CorporateBankAccountDTOs
{
    public class CorporateBankAccountDTO
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string CurrencyType { get; set; }
        public string CompanyOwnerShortName { get; set; }
        public string CompanyCode { get; set; }
        public string BankName { get; set; }
        public string RegistrationDate { get; set; }
        public ICollection<CompanyBankAssetDTO> BankAssets { get; set; } = new List<CompanyBankAssetDTO>();
    }
}
