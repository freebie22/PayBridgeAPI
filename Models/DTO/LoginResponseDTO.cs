namespace PayBridgeAPI.Models.DTO
{
    public class LoginResponseDTO
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
}
