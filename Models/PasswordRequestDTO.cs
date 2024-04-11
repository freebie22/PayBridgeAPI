namespace PayBridgeAPI.Models
{
    public class PasswordRequestDTO
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public string OldPassword { get; set; }
        public string Token { get; set; } = "";
    }
}
