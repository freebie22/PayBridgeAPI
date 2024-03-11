using PayBridgeAPI.Models.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class PersonalAccountHolderCreateDTO
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
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
        public string PassportSeries { get; set; }
        [Required]
        public string PassportNumber { get; set; }
        [Required]
        public string TaxIdentificationNumber { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
