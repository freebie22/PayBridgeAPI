using PayBridgeAPI.Models.User;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.MainModels
{
    public class PersonalBankAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }
        [Required]
        public string AccountNumber { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string AccountType { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public int AccountOwnerId { get; set; }
        [ForeignKey(nameof(AccountOwnerId))]
        public virtual PersonalAccountHolder AccountOwner { get; set; }
        [Required]
        public int BankId { get; set; }
        [ForeignKey(nameof(BankId))]
        public virtual Bank Bank { get; set; }
        public ICollection<BankCard> BankCards { get; set; } = new List<BankCard>();
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
