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
        public string Name { get; set; }
        [Required]
        public string RegistraionNumber { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string HeadquarterAddress { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public string ContactEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime EstablishedDate { get; set; }
        [Required]
        public string SWIFTCode { get; set; }
        [Required]
        public string CEOFirstName { get; set; }
        [Required]
        public string CEOLastName { get; set; }
        [Required]
        public string CEOMiddleName { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public ICollection<CorparateBankAccount> CorparateBankAccounts { get; set;}
        public ICollection<PersonalBankAccount> PersonalBankAccounts { get; set;}
    }
}
