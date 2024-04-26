using PayBridgeAPI.Models.MainModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PayBridgeAPI.Models.DTO.CorporateAccountHolderDTOs
{
    public class CorporateAccountHolderDTO
    {
        public int AccountId { get; set; }
        public string ShortCompanyName { get; set; }
        public string FullCompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public bool EmailConfirmed { get; set; }
        public string DateOfEstablishment { get; set; }
        public int PostalCode { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string StreetAddress { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
    }
}
