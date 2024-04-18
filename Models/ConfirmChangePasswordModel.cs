namespace PayBridgeAPI.Models
{
    public class ConfirmChangePasswordModel
    {
        public string Login { get; set; }
        public string PasswordToken { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set;}
    }
}
