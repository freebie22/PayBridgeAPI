using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class BankUpdateDTO
    {
        public int BankId { get; set; }
        public string FullBankName { get; set; }
        public string ShortBankName { get; set; }
        public string IBAN { get; set; }
        public string NationalStateRegistryNumber { get; set; }
        public string BankIdentificationCode { get; set; }
        public string SettlementAccount { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string NationalBankLicense { get; set; }
        public string SWIFTCode { get; set; }
        public string HeadquarterAddress { get; set; }
        public string EstablishedDate { get; set; }
        public string ContactNumber { get; set; }
        public string ContactEmail { get; set; }
        public string WebsiteURL { get; set; }
    }
}
