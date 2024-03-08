using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.MainModels
{
    public class CorparateBankAccount
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
        public decimal Balance { get; set; }
        [Required]
        public int BankId { get; set; }
        [ForeignKey(nameof(BankId))]
        public virtual Bank Bank { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
