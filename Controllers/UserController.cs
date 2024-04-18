using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.User;
using PayBridgeAPI.Repository.UserRepo;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PayBridgeAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        protected APIResponse _response;

        private static string GetShortenedToken(string token)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
                return Convert.ToBase64String(hashBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .TrimEnd('=');
            }
        }

        private static string DecodeShortenedToken(string shortenedToken)
        {
            int missingPadding = (4 - shortenedToken.Length % 4) % 4;
            shortenedToken = shortenedToken.PadRight(shortenedToken.Length + missingPadding, '=');

            shortenedToken = shortenedToken.Replace("-", "+").Replace("_", "/");

            byte[] hashBytes = Convert.FromBase64String(shortenedToken);

            return Encoding.UTF8.GetString(hashBytes);
        }
        

        public UserController(IUserRepository userRepository, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _emailSender = emailSender;
            _response = new APIResponse();
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Login([FromBody]LoginRequestDTO loginInfo)
        {
            try
            {
                if (loginInfo == null)
                {
                    throw new ArgumentNullException(nameof(loginInfo), "Error. Login info is null");
                }

                var loginResponse = await _userRepository.Login(loginInfo);

                if(!string.IsNullOrEmpty(loginResponse.ErrorMessage))
                {
                    throw new Exception(loginResponse.ErrorMessage);
                }

                _response.Result = loginResponse;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);


            }
            catch(Exception ex)
            {
                if(ex is ArgumentNullException)
                {
                    _response.ErrorMessages.Add(ex.Message);
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                if(ex is Exception)
                {
                    _response.ErrorMessages.Add(ex.Message);
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> Register([FromForm]RegistrationRequestDTO registerInfo)
        {
            try
            {
                if (registerInfo == null)
                {
                    throw new ArgumentNullException(nameof(registerInfo), "Error. Register info is null.");
                }

                var isUniqueLoginInfo = await _userRepository.IsUniqueLoginInfo(registerInfo.Username, registerInfo.Email);

                if(!isUniqueLoginInfo)
                {
                    throw new InvalidOperationException("Error. Username or email is already attached to another account.");
                }

                var registerResponse = await _userRepository.Register(registerInfo);

                if(registerInfo is null)
                {
                    throw new InvalidOperationException("Error. An error occured with registration.");
                }

                _response.Result = registerInfo;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);


            }
            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    _response.ErrorMessages.Add(ex.Message);
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost("ConfirmEmailRequest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> ConfirmEmailRequest([FromBody]EmailConfirmationRequest confirmModel)
        {
            try
            {
                if (confirmModel == null)
                {
                    throw new ArgumentException("Сталась помилка. Тіло запиту є null.");
                }

                var userQuery = await _userManager.FindByEmailAsync(confirmModel.Email);

                if(userQuery == null || userQuery.Id != confirmModel.UserId)
                {
                    throw new ArgumentException("При виконанні запиту сталась помилка. Користувача за вказаним E-Mail не існує, або він не належить даному користувачеві.");
                }

                if(userQuery.EmailConfirmed)
                {
                    throw new InvalidOperationException("Даний E-Email вже є підтвердженим");
                }

                var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(userQuery);

                confirmToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmToken));

                await _emailSender.SendEmailAsync(confirmModel.Email, "Підтвердження електронної пошти в сервісі PayBridge",
                    $@"<span>Вітаємо! Нами було отримано запит на підтвердження даної електронної пошти в нашому сервісі. Будь-ласка, перейдіть за <a href='http://localhost:5173/emailConfirmation/{confirmToken}'>посиланням</a> для підтвердження Вашої електронної пошти</span>");
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Запит на підтвердження електронної пошти надіслано на E-Mail вказаний при реєстрації.";

                return Ok(_response);
            }

            catch(Exception ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
        }

        [HttpPost("ConfirmEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> ConfirmEmail([FromBody]EmailConfirmation confirmation)
        {
            try
            {
                if (string.IsNullOrEmpty(confirmation.UserId) || string.IsNullOrEmpty(confirmation.Token))
                {
                    throw new ArgumentException("Сталась помилка. Серед параметрів запиту є null.");
                }

                var userQuery = await _userManager.FindByIdAsync(confirmation.UserId);

                if (userQuery == null )
                {
                    throw new ArgumentException("При виконанні запиту сталась помилка. Користувача не знайдено.");
                }


                var decodeToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(confirmation.Token));

                var result = await _userManager.ConfirmEmailAsync(userQuery, decodeToken);

                if(result.Succeeded)
                {
                    await _emailSender.SendEmailAsync(userQuery.Email, "Електронну пошту підтверджено в сервісі PayBridge",
                    $@"<span>Вітаємо! Ви успішно підтвердили електронну пошту в нашому сервісі. Дякуємо за співпрацю!");
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.Result = "Вітаємо! Ви успішно електронну пошту в нашому сервісі. Тепер Ви можете реєструвати власні рахунки та виконувати операції за ними.";

                    return Ok(_response);
                }

                else
                {
                    throw new InvalidOperationException("Сталась помилка з підтвердженням E-Mail");
                }
            }

            catch (Exception ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
        }


        [HttpPost("ForgotPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> ForgotPassword(string login)
        {
            try
            {
                if (string.IsNullOrEmpty(login))
                {
                    throw new ArgumentException("Сталась помилка. Тіло запиту є null");
                }


                ApplicationUser userByLogin = await _userManager.FindByNameAsync(login);
                if (userByLogin == null)
                {
                    userByLogin = await _userManager.FindByEmailAsync(login);
                    if (userByLogin == null)
                    {
                        throw new NullReferenceException("Помилка. Користувача за вказаним логіном не існує.");
                    }
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(userByLogin);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                await _emailSender.SendEmailAsync(userByLogin.Email, "Запит на скидання паролю", $@"Вітаємо! Ми отримали запит на скидання паролю з Вашого аккаунту. Перейдіть за <a href='http://localhost:5173/changePassword/{userByLogin.Email}/{token}'><h6>посиланням</h6></a> для подальшого процесу скидання паролю. Якщо Ви не робили такий запит, просто проігноруйте дане повідомлення");

                _response.Result = "E-Mail з скидання паролю було успішно надіслано.";
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            catch(Exception ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }

        [HttpPost("ChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> ChangePassword([FromBody]ChangePasswordModel passwordModel)
        {
            try
            {
                if (string.IsNullOrEmpty(passwordModel.Login))
                {
                    throw new ArgumentException("Сталась помилка. Тіло запиту є null");
                }

                ApplicationUser userByLogin = await _userManager.FindByNameAsync(passwordModel.Login);
                if (userByLogin == null)
                {
                    userByLogin = await _userManager.FindByEmailAsync(passwordModel.Login);
                    if (userByLogin == null)
                    {
                        throw new NullReferenceException("Помилка. Користувача за вказаним логіном не існує.");
                    }
                }
                bool passwordCheck;

                if (string.IsNullOrEmpty(passwordModel.PasswordToken) && !string.IsNullOrEmpty(passwordModel.OldPassword))
                {
                    passwordCheck = await _userManager.CheckPasswordAsync(userByLogin, passwordModel.OldPassword);

                    if (!passwordCheck)
                    {
                        throw new InvalidOperationException("Введений Вами пароль не правильний. Будь-ласка, перевірте його правильність");
                    }
                }

                else if(string.IsNullOrEmpty(passwordModel.PasswordToken) && string.IsNullOrEmpty(passwordModel.OldPassword))
                {
                    throw new InvalidOperationException("При зміні паролю сталась помилка.");
                }


                string token;

                if(string.IsNullOrEmpty(passwordModel.PasswordToken))
                {
                    token = await _userManager.GeneratePasswordResetTokenAsync(userByLogin);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                }

                else
                {
                    token = passwordModel.PasswordToken;
                }

                PasswordChangeDTO passwordChangeDTO = new PasswordChangeDTO()
                {
                    Login = passwordModel.Login,
                    Token = token,
                };

                _response.Result = passwordChangeDTO;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            catch (Exception ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }

        [HttpPost("ConfirmChangePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> ConfirmChangePassword([FromBody] ConfirmChangePasswordModel passwordModel) 
        {
            try
            {
                if (string.IsNullOrEmpty(passwordModel.Login))
                {
                    throw new ArgumentException("Сталась помилка. Тіло запиту є null");
                }


                ApplicationUser userByLogin = await _userManager.FindByNameAsync(passwordModel.Login);
                if (userByLogin == null)
                {
                    userByLogin = await _userManager.FindByEmailAsync(passwordModel.Login);
                    if (userByLogin == null)
                    {
                        throw new NullReferenceException("Помилка. Користувача за вказаним логіном не існує.");
                    }
                }


                var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(passwordModel.PasswordToken));

                if(!string.Equals(passwordModel.NewPassword, passwordModel.ConfirmPassword))
                {
                    throw new InvalidOperationException("Паролі повинні співпадати.");
                }

                if(await _userManager.CheckPasswordAsync(userByLogin, passwordModel.NewPassword))
                {
                    throw new InvalidOperationException("Будь-ласка, застосуйте інший пароль, адже він вже був використаний.");
                }

                var result = await _userManager.ResetPasswordAsync(userByLogin, token, passwordModel.NewPassword);

                if(result.Succeeded)
                {
                    _response.Result = "Пароль успішно змінено";
                    _response.StatusCode = HttpStatusCode.OK;
                    await _emailSender.SendEmailAsync(userByLogin.Email, "Пароль успішно змінено!", $@"<h5>Пароль Вашого облікового запису було успішно змінено.</h5>");
                    return Ok(_response);
                }

                else
                {
                    throw new InvalidOperationException("При зміні паролю сталась помилка.");
                }

            }

            catch (Exception ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }




    }
}
