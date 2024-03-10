using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class BankCreateDTO
    {
        [Required]
        public string FullBankName { get; set; }
        [Required]
        public string ShortBankName { get; set; }
        [Required]
        public string IBAN { get; set; }
        [Required]
        public string NationalStateRegistryNumber { get; set; }
        [Required]
        public string BankIdentificationCode { get; set; }
        [Required]
        public string SettlementAccount { get; set; }
        [Required]
        public string TaxIdentificationNumber { get; set; }
        [Required]
        public string NationalBankLicense { get; set; }
        [Required]
        public string SWIFTCode { get; set; }
        [Required]
        public string HeadquarterAddress { get; set; }
        [Required]
        public string EstablishedDate { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        [Required]
        public string WebsiteURL { get; set; }

    }
}
