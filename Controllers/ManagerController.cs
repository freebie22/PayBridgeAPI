using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Models.User;
using PayBridgeAPI.Repository;
using System.Net;
using System.Text;

namespace PayBridgeAPI.Controllers
{
    [Route("api/Manager")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly IManagerRepository _repository;
        private readonly ILogger<IManagerRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private APIResponse _response;

        public ManagerController(IManagerRepository repository, ILogger<IManagerRepository> logger, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _logger = logger;
            _userManager = userManager;
            _response = new APIResponse();
        }

        [HttpGet("GetManagers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetManagers()
        {
            try
            {
                var query = await _repository.GetAllValues(include: q => q.Include(q => q.User));

                if(query.Count == 0)
                {
                    throw new NullReferenceException("Error. No managers have been found in database");
                }

                IList<ManagerDTO> managers = new List<ManagerDTO>();
                foreach(Manager manager in query)
                {
                    managers.Add(new ManagerDTO()
                    {
                        ManagerId = manager.ManagerId,
                        FirstName = manager.FirstName,
                        LastName = manager.LastName,
                        MiddleName = manager.MiddleName,
                        Email = manager.User.Email,
                        EmailConfirmed = manager.User.EmailConfirmed,
                        PhoneNumber = manager.User.PhoneNumber,
                        Position = manager.Position,
                        IsActive = manager.IsActive,
                        Description = manager.Description,
                        ProfileImage = Encoding.ASCII.GetString(manager.User.ProfileImage)
                    });
                }

                _response.Result = managers;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }

            catch(NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message);
                return BadRequest(_response);
            }
        }

        [HttpGet("GetManager/{id:maxlength(36)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetManager(string id)
        {
            try
            {
                var query = await _repository.GetValueAsync(filter: q => q.UserId == id, include: q => q.Include(q => q.User));

                if (query == null)
                {
                    throw new NullReferenceException($"Error. No managers have been found in database by id {id}");
                }

                ManagerDTO manager = new ManagerDTO()
                {
                    ManagerId = query.ManagerId,
                    FirstName = query.FirstName,
                    LastName = query.LastName,
                    MiddleName = query.MiddleName,
                    EmailConfirmed = query.User.EmailConfirmed,
                    Email = query.User.Email,
                    PhoneNumber = query.User.PhoneNumber,
                    Position = query.Position,
                    IsActive = query.IsActive,
                    Description = query.Description,
                    ProfileImage = Encoding.ASCII.GetString(query.User.ProfileImage)
                };

                _response.Result = manager;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message);
                return BadRequest(_response);
            }
        }


        [HttpPost("CreateManager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateManager([FromBody] ManagerCreateDTO managerDTO)
        {
            try
            {
                if (managerDTO == null)
                {
                    throw new ArgumentNullException(nameof(managerDTO), "Error. Manager parameter was null");
                }

                var isUniqueUserId = await _repository.GetValueAsync(i => i.UserId == managerDTO.UserId);

                if (isUniqueUserId != null)
                {
                    throw new InvalidOperationException("Error. Manager with this userId is already exists. Please, choose another user, to create new manager");
                }

                Manager manager = new Manager()
                {
                    FirstName = managerDTO.FirstName,
                    LastName = managerDTO.LastName,
                    MiddleName = managerDTO.MiddleName,
                    Position = managerDTO.Position,
                    Description = managerDTO.Description,
                    IsActive = managerDTO.IsActive,
                    UserId = managerDTO.UserId,
                };

                await _repository.CreateAsync(manager);

                _response.Result = manager;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }

            catch(Exception ex)
            {
                if(ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(ex.Message);
                    return BadRequest(_response);
                }
                else
                {
                    throw;
                }
            }
            
        }

        [HttpPut("UpdateManager/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateManager(int id, [FromForm] ManagerUpdateDTO managerDTO)
        {
            try
            {
                if (managerDTO == null)
                {
                    throw new ArgumentNullException(nameof(managerDTO), "Error. Manager parameter was null");
                }

                if(id == 0 || id != managerDTO.ManagerId)
                {
                    throw new ArgumentException("Error. Id of request and id of manager dont correspond.");
                }

                var existingManager = await _repository.GetValueAsync(filter: m => m.ManagerId == id, isTracked: false, include: q => q.Include(q => q.User));

                if(existingManager == null)
                {
                    throw new ArgumentException($"Error. Manager with id of {id} doesn't exist");
                }

                Manager manager = new Manager()
                {
                    ManagerId = managerDTO.ManagerId,
                    FirstName = managerDTO.FirstName,
                    LastName = managerDTO.LastName,
                    MiddleName = managerDTO.MiddleName,
                    Position = managerDTO.Position,
                    IsActive = managerDTO.IsActive,
                    Description = managerDTO.Description,
                    HireDate = existingManager.HireDate,
                    UserId = existingManager.UserId,
                };

                
                await _repository.UpdateAsync(manager);
                await _repository.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return NoContent();
            }

            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(ex.Message);
                    return BadRequest(_response);
                }
                else
                {
                    throw;
                }
            }

        }

        [HttpPatch("UpdateManagerPartially/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateManagerPartially(int id, [FromBody] JsonPatchDocument<ManagerUpdateDTO> patchDTO)
        {
            try
            {

                var existingManager = await _repository.GetValueAsync(filter: m => m.ManagerId == id, isTracked: false);

                if (existingManager == null)
                {
                    throw new ArgumentException($"Error. Manager with id of {id} doesn't exist");
                }

                ManagerUpdateDTO manager = new ManagerUpdateDTO()
                {
                    ManagerId = existingManager.ManagerId,
                    FirstName = existingManager.FirstName,
                    LastName = existingManager.LastName,
                    MiddleName = existingManager.MiddleName,
                    Position = existingManager.Position,
                    IsActive = existingManager.IsActive,
                    Description = existingManager.Description
                };

                patchDTO.ApplyTo(manager);

                Manager updateManager = new Manager()
                {
                    ManagerId = manager.ManagerId,
                    FirstName = manager.FirstName,
                    LastName = manager.LastName,
                    MiddleName = manager.MiddleName,
                    Position = manager.Position,
                    IsActive = manager.IsActive,
                    Description = manager.Description,
                    UserId = existingManager.UserId
                };


                await _repository.UpdateAsync(updateManager);
                await _repository.SaveChangesAsync();

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return NoContent();
            }

            catch (Exception ex)
            {
                if (ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.ErrorMessages.Add(ex.Message);
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
