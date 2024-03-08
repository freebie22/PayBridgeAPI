using Stripe;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.MainModels
{
    public class BankCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BankCardId { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public int CVC { get; set; }
        [Required]
        public string CurrencyType { get; set; }
        [Required]
        public decimal Balance { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public int BankAccountId { get; set; }

        [ForeignKey(nameof(BankAccountId))]
        public virtual PersonalBankAccount Account { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
