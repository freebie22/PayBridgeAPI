namespace PayBridgeAPI.Models.DTO.ResponsiblePeopleDTOs
{
    public class ResponsiblePersonDTO
    {
        public int ResponsiblePersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public string PositionInCompany { get; set; }
        public bool IsActive { get; set; }

    }
}
