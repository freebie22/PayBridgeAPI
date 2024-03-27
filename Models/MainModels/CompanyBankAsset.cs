using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.MainModels
{
    public class CompanyBankAsset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetId { get; set; }
        [Required]
        public string AssetUniqueNumber { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string IBAN_Number { get; set; }
        [Required]
        public string CurrencyType { get; set; }
        [Required]
        public decimal Balance { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int CorporateAccountId { get; set; }
        [ForeignKey(nameof(CorporateAccountId))]
        public virtual CorporateBankAccount CorporateAccount { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
