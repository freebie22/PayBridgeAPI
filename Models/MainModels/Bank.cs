using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.MainModels
{
    public class Bank
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BankId { get; set; }
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
        public string RegistraionNumber { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string HeadquarterAddress { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        [Required]
        public string WebsiteURL { get; set; }
        public bool EmailConfirmed { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime EstablishedDate { get; set; }
        [Required]
        public string SWIFTCode { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public ICollection<CorporateBankAccount> CorporateBankAccounts { get; set;} = new List<CorporateBankAccount>();
        public ICollection<PersonalBankAccount> PersonalBankAccounts { get; set;} = new List<PersonalBankAccount>();
    }
}
