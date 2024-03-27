using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.MainModels
{
    public class CorporateBankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }
        [Required]
        public string AccountNumber { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string AccountType { get; set; }
        [Required]
        public string CurrencyType { get; set; }
        [Required]
        public int AccountOwnerId { get; set; }
        [ForeignKey(nameof(AccountOwnerId))]
        public virtual CorporateAccountHolder AccountOwner { get; set; }
        [Required]
        public int ManagerId { get; set; }
        [ForeignKey(nameof(ManagerId))]
        public virtual Manager Manager { get; set; }
        [Required]
        public int BankId { get; set; }
        [ForeignKey(nameof(BankId))]
        public virtual Bank Bank { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        [Required]
        public bool IsActive { get; set; } = false;
        [Required]
        public string Status { get; set; } 
        public ICollection<CompanyBankAsset> BankAssets { get; set; } = new List<CompanyBankAsset>();
    }
}
