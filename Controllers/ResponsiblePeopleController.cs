using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.DTO.ResponsiblePeopleDTOs;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Models.User;
using PayBridgeAPI.Repository;
using PayBridgeAPI.Repository.ResponsiblePeopleRepo;
using Stripe;
using System;
using System.Drawing;
using System.Net;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PayBridgeAPI.Controllers
{
    [Route("api/ResponsiblePeople")]
    [ApiController]
    public class ResponsiblePeopleController : ControllerBase
    {
        private readonly IResponsiblePersonRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private APIResponse _response;

        public ResponsiblePeopleController(IResponsiblePersonRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
            _response = new APIResponse();
        }

        [HttpGet("GetResponsiblePeople")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetResponsiblePeople()
        {
            try
            {
                var query = await _repository.GetAllValues(include: q => q.Include(q => q.User));

                if(query.Count == 0)
                {
                    throw new NullReferenceException("Жодної відповідальної особи не було знайдено в базі даних.");
                }

                IList<ResponsiblePersonDTO> responsiblePeople = new List<ResponsiblePersonDTO>();
                foreach(ResponsiblePerson person in query)
                {
                    responsiblePeople.Add(new ResponsiblePersonDTO()
                    {
                        ResponsiblePersonId = person.ResponsiblePersonId,
                        FirstName = person.FirstName,
                        LastName = person.LastName,
                        MiddleName = person.MiddleName,
                        Email = person.User.Email,
                        EmailConfirmed = person.User.EmailConfirmed,
                        PhoneNumber = person.User.PhoneNumber,
                        PositionInCompany = person.PositionInCompany,
                        IsActive = person.IsActive,
                    });
                }

                _response.Result = responsiblePeople;
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

        [HttpGet("GetResponsiblePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetResponsiblePerson([FromQuery]string userId)
        {
            try
            {
                var query = await _repository.GetValueAsync(filter: q => q.UserId == userId, include: q => q.Include(q => q.User));

                if (query == null)
                {
                    throw new NullReferenceException($"Error. No managers have been found in database by id {userId}");
                }

                ResponsiblePersonDTO person = new ResponsiblePersonDTO()
                {
                    ResponsiblePersonId = query.ResponsiblePersonId,
                    FirstName = query.FirstName,
                    LastName = query.LastName,
                    MiddleName = query.MiddleName,
                    Email = query.User.Email,
                    EmailConfirmed = query.User.EmailConfirmed,
                    PhoneNumber = query.User.PhoneNumber,
                    PositionInCompany = query.PositionInCompany,
                    IsActive = query.IsActive,
                };

                _response.Result = person;
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


        [HttpPost("CreateResponsiblePerson")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateResponsiblePerson([FromBody] ResponsiblePersonCreateDTO personDTO)
        {
            try
            {
                if (personDTO == null)
                {
                    throw new ArgumentNullException(nameof(personDTO), "Error. ResponsiblePerson parameter was null");
                }

                ApplicationUser existingUser = null;

                existingUser = await _userManager.FindByNameAsync(personDTO.Login);

                if (existingUser == null)
                {
                    existingUser = await _userManager.FindByEmailAsync(personDTO.Login);

                    if(existingUser == null)
                    {
                        throw new NullReferenceException("Користувача за даним логіном або E-Mail не знайдено");
                    }
                }

                var isUniqueUserId = await _repository.GetValueAsync(i => i.UserId == existingUser.Id);

                if (isUniqueUserId != null)
                {
                    throw new InvalidOperationException("Помилка. Відповідальна особа з даним ідентифікатором користувача вже існує.");
                }

                ResponsiblePerson person = new ResponsiblePerson()
                {
                    FirstName = personDTO.FirstName,
                    LastName = personDTO.LastName,
                    MiddleName = personDTO.MiddleName,
                    PositionInCompany = personDTO.PositionInCompany,
                    IsActive = personDTO.IsActive,
                    UserId = existingUser.Id,
                };

                await _repository.CreateAsync(person);

                _response.Result = person;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
            }

            catch(Exception ex)
            {
                if(ex is ArgumentNullException || ex is InvalidOperationException || ex is NullReferenceException)
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

        [HttpPut("UpdateResponsiblePerson/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateResponsiblePerson(int id, [FromBody] ResponsiblePersonUpdateDTO personDTO)
        {
            try
            {
                if (personDTO == null)
                {
                    throw new ArgumentNullException(nameof(personDTO), "Error. Manager parameter was null");
                }

                if(id == 0 || id != personDTO.ResponsiblePersonId)
                {
                    throw new ArgumentException("Error. Id of request and id of manager dont correspond.");
                }

                var existingPerson = await _repository.GetValueAsync(filter: m => m.ResponsiblePersonId == id, isTracked: false, include: q => q.Include(q => q.User));

                if(existingPerson == null)
                {
                    throw new ArgumentException($"Помилка. Відповідальну особу за Вашим запитом не знайдено");
                }

                ResponsiblePerson person = new ResponsiblePerson()
                {
                    ResponsiblePersonId = existingPerson.ResponsiblePersonId,
                    FirstName = personDTO.FirstName,
                    LastName = personDTO.LastName,
                    MiddleName = personDTO.MiddleName,
                    PositionInCompany = personDTO.PositionInCompany,
                    IsActive = personDTO.IsActive,
                    UserId = existingPerson.UserId,
                };

                
                await _repository.UpdateResponsiblePerson(person);
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

        [HttpPatch("UpdateResponsiblePersonPartially/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateManagerPartially(int id, [FromBody] JsonPatchDocument<ResponsiblePersonUpdateDTO> patchDTO)
        {
            try
            {

                var existingPerson = await _repository.GetValueAsync(filter: m => m.ResponsiblePersonId == id, isTracked: false);

                if (existingPerson == null)
                {
                    throw new ArgumentException($"Error. Manager with id of {id} doesn't exist");
                }

                ResponsiblePersonUpdateDTO person = new ResponsiblePersonUpdateDTO()
                {
                    ResponsiblePersonId = existingPerson.ResponsiblePersonId,
                    FirstName = existingPerson.FirstName,
                    LastName = existingPerson.LastName,
                    MiddleName = existingPerson.MiddleName,
                    PositionInCompany = existingPerson.PositionInCompany,
                    IsActive = existingPerson.IsActive,
                };

                patchDTO.ApplyTo(person);

                ResponsiblePerson updatedPerson = new ResponsiblePerson()
                {
                    ResponsiblePersonId = existingPerson.ResponsiblePersonId,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    MiddleName = person.MiddleName,
                    PositionInCompany = person.PositionInCompany,
                    IsActive = person.IsActive,
                    UserId = existingPerson.UserId,
                };


                await _repository.UpdateResponsiblePerson(updatedPerson);
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
