namespace PayBridgeAPI.Models
{
    public class ChangePasswordModel
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public string OldPassword { get; set; } = "";
        public string PasswordToken { get; set; } = "";
    }
}
