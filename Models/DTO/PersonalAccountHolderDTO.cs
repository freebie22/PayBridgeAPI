using System.ComponentModel.DataAnnotations.Schema;

namespace PayBridgeAPI.Models.DTO
{
    public class PersonalAccountHolderDTO
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string TaxIdentificationNumber { get; set; }
        public string ProfileImage { get; set; }
    }
}
