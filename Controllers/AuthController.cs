using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Repository.UserRepo;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        protected APIResponse _response;
        

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }
}
