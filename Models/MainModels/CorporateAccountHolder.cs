using PayBridgeAPI.Models.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.MainModels
{
    public class CorporateAccountHolder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }
        [Required]
        public string ShortCompanyName { get; set; }
        [Required]
        public string FullCompanyName { get; set; }
        [Required]
        public string EnglishName { get; set; }
        [Required] 
        public string CompanyCode { get; set; }
        [Required] 
        public string ContactEmail { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [Required]
        public bool EmailConfirmed { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateOfEstablishment { get; set; }
        [Required]
        public int PostalCode { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public ICollection<string> TypesOfActivity { get; set; }
        [Required]
        public int ManagerId { get; set; }
        [Required]
        public virtual Manager Manager { get; set; }
    }
}
