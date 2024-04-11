using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO
{
    public class PersonalAccountHolderUpdateDTO
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public int PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public bool IsActive { get; set; }  
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string TaxIdentificationNumber { get; set; }
    }
}
