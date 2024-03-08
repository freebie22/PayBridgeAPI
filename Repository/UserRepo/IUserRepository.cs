using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;

namespace PayBridgeAPI.Repository.UserRepo
{
    public interface IUserRepository
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO loginInfo);
        Task<UserDTO> Register(RegistrationRequestDTO registerInfo);
        Task<bool> IsUniqueLoginInfo(string userName, string email);

    }
}
