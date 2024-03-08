using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PayBridgeAPI.Data;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.User;
using PayBridgeAPI.Services.AzureBlobs;
using PayBridgeAPI.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PayBridgeAPI.Repository.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;
        private string _apiKey;

        public UserRepository(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, IBlobService blobService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _apiKey = configuration.GetValue<string>("ApiSettings:Secret");
            _blobService = blobService;
        }

        public async Task<bool> IsUniqueLoginInfo(string userName, string email)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if(user == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginInfo)
        {
            var user = await _userManager.FindByNameAsync(loginInfo.Login);

            if(user == null)
            {
                user = await _userManager.FindByEmailAsync(loginInfo.Login);
                if(user == null)
                {
                    LoginResponseDTO loginResponseDTO = new LoginResponseDTO() {
                        ErrorMessage = "User hasn't been found. Check your login info one more time."
                    };

                    return loginResponseDTO;
                }
            }

            bool correctPassword = await _userManager.CheckPasswordAsync(user, loginInfo.Password);
            if (!correctPassword)
            {
                LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                {
                    ErrorMessage = "Password is not correct."
                };
                return loginResponseDTO;
            }

            var userRole = await _userManager.GetRolesAsync(user);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] secretKey = Encoding.ASCII.GetBytes(_apiKey);

            SecurityTokenDescriptor tokenDesriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("phoneNumber", user.PhoneNumber),
                    new Claim(ClaimTypes.Role, userRole.First())
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.Now.AddDays(7)
            };

            var jwtToken = tokenHandler.CreateToken(tokenDesriptor);

            LoginResponseDTO responseDTO = new LoginResponseDTO()
            {
                Email = user.Email,
                Token = tokenHandler.WriteToken(jwtToken),
                ErrorMessage = ""
            };

            return responseDTO;

        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registerInfo)
        {
            var user = new ApplicationUser()
            {
                UserName = registerInfo.Username,
                Email = registerInfo.Email,
                NormalizedEmail = registerInfo.Email.ToUpper(),
                PhoneNumber = registerInfo.PhoneNumber,
            };

            if(registerInfo.ProfileImage != null)
            {
                string image = $"{Guid.NewGuid()}{Path.GetExtension(registerInfo.ProfileImage.FileName)}";
                user.ProfileImage = Encoding.ASCII.GetBytes(await _blobService.UpdateBlob(image, registerInfo.ProfileImage));
            }


            var result = await _userManager.CreateAsync(user, registerInfo.Password);
            if(result.Succeeded)
            {
                if(!await _roleManager.RoleExistsAsync(Roles.ADMIN_ROLE))
                {
                    await _roleManager.CreateAsync(new IdentityRole(Roles.ADMIN_ROLE));
                    await _roleManager.CreateAsync(new IdentityRole(Roles.MANAGER_ROLE));
                    await _roleManager.CreateAsync(new IdentityRole(Roles.CLIENT_ROLE));
                }

                switch(registerInfo.Role)
                {
                    case Roles.ADMIN_ROLE:
                        await _userManager.AddToRoleAsync(user, registerInfo.Role);
                        break;
                    case Roles.MANAGER_ROLE:
                        await _userManager.AddToRoleAsync(user, registerInfo.Role);
                        break;
                    case Roles.CLIENT_ROLE:
                        await _userManager.AddToRoleAsync(user, registerInfo.Role);
                        break;
                }

                var newUser = new UserDTO()
                {
                    Email = user.Email,
                    PhoneNumber = registerInfo.PhoneNumber,
                    Username = registerInfo.Username,
                    ProfileImage = Encoding.ASCII.GetString(user.ProfileImage) ?? "No Image"
                };

                return newUser;

                
            }
            else
            {
                return null;
            }
        }
    }
}
