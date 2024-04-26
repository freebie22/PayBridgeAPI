using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Data;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.DTO.CorporateAccountHolderDTOs;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Models.User;
using PayBridgeAPI.Repository;
using PayBridgeAPI.Repository.ResponsiblePeopleRepo;
using Stripe.Climate;
using System.Globalization;
using System.Net;
using System.Text;

namespace PayBridgeAPI.Controllers
{
    [Route("api/accountHolder")]
    [ApiController]
    public class AccountHolderController : ControllerBase
    {
        private readonly IPersonalAccountRepository _personalRepo;
        private readonly ICorporateAccountRepository _corporateRepo;
        private readonly IResponsiblePersonRepository _responsibleRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly PayBridgeDbContext _context;
        protected APIResponse _response;

        public AccountHolderController(IPersonalAccountRepository personalRepo, ICorporateAccountRepository corporateRepo, IResponsiblePersonRepository responsibleRepo, PayBridgeDbContext context, UserManager<ApplicationUser> userManager)
        {
            _personalRepo = personalRepo;
            _corporateRepo = corporateRepo;
            _responsibleRepo = responsibleRepo;
            _context = context;
            _userManager = userManager;
            _response = new APIResponse();
        }

        [HttpGet("GetPersonalAccountHolders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPersonalAccountHolders([FromQuery]string holderUserId = "")
        {
            try
            {
                IList<PersonalAccountHolder> query;

                if(!string.IsNullOrEmpty(holderUserId))
                {
                    query = await _personalRepo.GetAllValues(include: q => q.Include(q => q.User), filter: q => q.UserId == holderUserId);
                }
                else
                {
                    query = await _personalRepo.GetAllValues(include: q => q.Include(q => q.User));
                }

                if (query.Count == 0)
                {
                    throw new NullReferenceException("Error. No personal account holders have been found in database");
                }

                IList<PersonalAccountHolderDTO> holders = new List<PersonalAccountHolderDTO>();
                foreach (PersonalAccountHolder holder in query)
                {
                    holders.Add(new PersonalAccountHolderDTO()
                    {
                       AccountId = holder.AccountId,
                       FirstName = holder.FirstName,
                       LastName = holder.LastName,
                       MiddleName = holder.MiddleName ?? "Не вказано",
                       DateOfBirth = holder.DateOfBirth.ToLongDateString(),
                       Email = holder.User.Email,
                       EmailConfirmed = holder.User.EmailConfirmed,
                       PhoneNumber = holder.User.PhoneNumber,
                       PostalCode = holder.PostalCode.ToString(),
                       Country = holder.Country,
                       State = holder.State,
                       City = holder.City,
                       StreetAddress = holder.StreetAddress,
                       PassportSeries = holder.PassportSeries ?? "ID-картка",
                       PassportNumber = holder.PassportNumber,
                       TaxIdentificationNumber = holder.TaxIdentificationNumber,
                       ProfileImage = /*Encoding.ASCII.GetString(holder.User.ProfileImage) != null ? Encoding.ASCII.GetString(holder.User.ProfileImage) :*/ "No Image"
                    });
                }

                _response.Result = holders;
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

        [HttpGet("GetPersonalAccountHolder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPersonalAccountHolder([FromQuery]string userId)
        {
            try
            {
                if(string.IsNullOrEmpty(userId))
                {
                    throw new NullReferenceException("Error. Query parameter cannot be null or empty.");
                }

                var holder = await _personalRepo.GetValueAsync(filter: a => a.UserId == userId, include: q => q.Include(q => q.User));

                if (holder == null)
                {
                    throw new NullReferenceException($"Error. No account holders have been found in database by userd id {userId}");
                }

                PersonalAccountHolderDTO account = new PersonalAccountHolderDTO()
                {
                    AccountId = holder.AccountId,
                    FirstName = holder.FirstName,
                    LastName = holder.LastName,
                    MiddleName = holder.MiddleName ?? "Не вказано",
                    DateOfBirth = holder.DateOfBirth.ToLongDateString(),
                    Email = holder.User.Email,
                    EmailConfirmed = holder.User.EmailConfirmed,
                    PhoneNumber = holder.User.PhoneNumber,
                    PostalCode = holder.PostalCode.ToString(),
                    Country = holder.Country,
                    State = holder.State,
                    City = holder.City,
                    StreetAddress = holder.StreetAddress,
                    PassportSeries = holder.PassportSeries ?? "ID-картка",
                    PassportNumber = holder.PassportNumber,
                    TaxIdentificationNumber = holder.TaxIdentificationNumber,
                    ProfileImage = /*Encoding.ASCII.GetString(holder.User.ProfileImage)*/ "No Image"
                };

                _response.Result = account;
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

        [HttpPost("CreatePersonalAccountHolder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreatePersonalAccountHolder([FromBody] PersonalAccountHolderCreateDTO holderDTO)
        {
            try
            {
                if (holderDTO == null)
                {
                    throw new ArgumentNullException(nameof(holderDTO), "Error. Request body was null");
                }

                var existingAccount = await _personalRepo.GetValueAsync(e => e.UserId == holderDTO.UserId);

                if (existingAccount != null)
                {
                    throw new InvalidOperationException("Error. Account holder with this userId is already exists. Please, choose another user, to create a new account holder.");
                }

                PersonalAccountHolder accountHolder = new PersonalAccountHolder()
                {
                    FirstName = holderDTO.FirstName,
                    LastName = holderDTO.LastName,
                    MiddleName = holderDTO.MiddleName ?? "Не вказано",
                    DateOfBirth = DateTime.ParseExact(holderDTO.DateOfBirth, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    PostalCode = holderDTO.PostalCode,
                    Country = holderDTO.Country,
                    State = holderDTO.State,
                    City = holderDTO.City,
                    StreetAddress = holderDTO.StreetAddress,
                    PassportSeries = holderDTO.PassportSeries ?? "ID-картка",
                    PassportNumber = holderDTO.PassportNumber,
                    TaxIdentificationNumber = holderDTO.TaxIdentificationNumber,
                    UserId = holderDTO.UserId,
                    IsActive = false
                };

                await _personalRepo.CreateAsync(accountHolder);
                await _personalRepo.SaveChanges();

                _response.Result = accountHolder;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
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


        [HttpPut("UpdatePersonalAccountHolder/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePersonalAccountHolder(int id, [FromBody] PersonalAccountHolderUpdateDTO accountDTO)
        {
            try
            {
                if (accountDTO == null)
                {
                    throw new ArgumentNullException(nameof(accountDTO), "Error. Request body was null");
                }

                if (id == 0 || id != accountDTO.AccountId)
                {
                    throw new ArgumentException("Error. Id of request and id of account don't correspond.");
                }

                var existingAccount = await _personalRepo.GetValueAsync(filter: m => m.AccountId == id, isTracked: false, include: q => q.Include(q => q.User));

                if (existingAccount == null)
                {
                    throw new ArgumentException($"Error. Account with id of {id} doesn't exist");
                }

                PersonalAccountHolder accountHolder = new PersonalAccountHolder()
                {
                    FirstName = accountDTO.FirstName,
                    LastName = accountDTO.LastName,
                    MiddleName = accountDTO.MiddleName ?? "Не вказано",
                    DateOfBirth = DateTime.ParseExact(accountDTO.DateOfBirth, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    PostalCode = accountDTO.PostalCode,
                    Country = accountDTO.Country,
                    State = accountDTO.State,
                    City = accountDTO.City,
                    StreetAddress = accountDTO.StreetAddress,
                    PassportSeries = accountDTO.PassportSeries ?? "ID-картка",
                    PassportNumber = accountDTO.PassportNumber,
                    TaxIdentificationNumber = accountDTO.TaxIdentificationNumber,
                    UserId = existingAccount.UserId,
                    IsActive = false
                };


                await _personalRepo.UpdateAccount(accountHolder);
                await _personalRepo.SaveChanges();

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

        [HttpPatch("UpdatePersonalAccountHolderPartially/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdatePersonalAccountHolderPartially(int id, [FromBody] JsonPatchDocument<PersonalAccountHolderUpdateDTO> patchDTO)
        {
            try
            {

                var existingAccount = await _personalRepo.GetValueAsync(filter: m => m.AccountId == id, include: m => m.Include(m => m.User), isTracked: false);

                if (existingAccount == null)
                {
                    throw new ArgumentException($"Error. Account with id of {id} doesn't exist");
                }

                var accountUser = await _userManager.FindByIdAsync(existingAccount.UserId);

                PersonalAccountHolderUpdateDTO updateDTO = new PersonalAccountHolderUpdateDTO()
                {
                    FirstName = existingAccount.FirstName,
                    LastName = existingAccount.LastName,
                    MiddleName = existingAccount.MiddleName,
                    DateOfBirth = existingAccount.DateOfBirth.ToLongDateString(),
                    PhoneNumber = existingAccount.User.PhoneNumber,
                    PostalCode = existingAccount.PostalCode,
                    Country = existingAccount.Country,
                    State = existingAccount.State,
                    City = existingAccount.City,
                    StreetAddress = existingAccount.StreetAddress,
                    PassportNumber = existingAccount.PassportNumber,
                    PassportSeries = existingAccount.PassportSeries,
                    TaxIdentificationNumber = existingAccount.TaxIdentificationNumber,
                };

                patchDTO.ApplyTo(updateDTO);

                PersonalAccountHolder accountHolder = new PersonalAccountHolder()
                {
                    AccountId = existingAccount.AccountId,
                    FirstName = updateDTO.FirstName,
                    LastName = updateDTO.LastName,
                    MiddleName = updateDTO.MiddleName ?? "Не вказано",
                    DateOfBirth = DateTime.ParseExact($"0{updateDTO.DateOfBirth}", "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    PostalCode = updateDTO.PostalCode,
                    Country = updateDTO.Country,
                    State = updateDTO.State,
                    City = updateDTO.City,
                    StreetAddress = updateDTO.StreetAddress,
                    PassportSeries = updateDTO.PassportSeries ?? "ID-картка",
                    PassportNumber = updateDTO.PassportNumber,
                    TaxIdentificationNumber = updateDTO.TaxIdentificationNumber,
                    UserId = existingAccount.UserId,
                    IsActive = updateDTO.IsActive
                };

                accountUser.PhoneNumber = updateDTO.PhoneNumber;

                await _userManager.UpdateAsync(accountUser);
                await _context.SaveChangesAsync();

                await _personalRepo.UpdateAccount(accountHolder);
                await _personalRepo.SaveChanges();

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

        [HttpGet("GetCorporateAccountHolders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCorporateAccountHolders()
        {
            try
            {
                var query = await _corporateRepo.GetAllValues(include: q => q.Include(q => q.ResponsiblePerson));

                if (query.Count == 0)
                {
                    throw new NullReferenceException("Error. No corporate account holders have been found in database");
                }

                IList<CorporateAccountHolderDTO> holders = new List<CorporateAccountHolderDTO>();
                foreach (CorporateAccountHolder holder in query)
                {
                    holders.Add(new CorporateAccountHolderDTO()
                    {
                       AccountId = holder.AccountId,
                       ShortCompanyName = holder.ShortCompanyName,
                       FullCompanyName = holder.FullCompanyName,
                       CompanyCode = holder.CompanyCode,
                       ContactEmail = holder.ContactEmail,
                       ContactPhone = holder.ContactPhone,
                       EmailConfirmed = holder.EmailConfirmed,
                       DateOfEstablishment = holder.DateOfEstablishment.ToLongDateString(),
                       PostalCode = holder.PostalCode,
                       Country = holder.Country,
                       State = holder.State,
                       City = holder.City,
                       StreetAddress = holder.StreetAddress,
                       IsActive = holder.IsActive,
                       Status = holder.Status
                    });
                }

                _response.Result = holders;
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

        [HttpGet("GetCorporateAccountHolder/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCorporateAccountHolder(int id)
        {
            try
            {
                var holder = await _corporateRepo.GetValueAsync(filter: a => a.AccountId == id, include: q => q.Include(q => q.ResponsiblePerson));

                if (holder == null)
                {
                    throw new NullReferenceException($"Error. No account holders have been found in database by id {id}");
                }

                CorporateAccountHolderDTO account = new CorporateAccountHolderDTO()
                {
                    AccountId = holder.AccountId,
                    ShortCompanyName = holder.ShortCompanyName,
                    FullCompanyName = holder.FullCompanyName,
                    CompanyCode = holder.CompanyCode,
                    ContactEmail = holder.ContactEmail,
                    ContactPhone = holder.ContactPhone,
                    EmailConfirmed = holder.EmailConfirmed,
                    DateOfEstablishment = holder.DateOfEstablishment.ToLongDateString(),
                    PostalCode = holder.PostalCode,
                    Country = holder.Country,
                    State = holder.State,
                    City = holder.City,
                    StreetAddress = holder.StreetAddress,
                    IsActive = holder.IsActive,
                    Status = holder.Status
                };

                _response.Result = account;
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

        [HttpPost("CreateCorporateAccountHolder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateCorporateAccountHolder([FromBody] CorporateAccountHolderCreateDTO holderDTO)
        {
            try
            {
                if (holderDTO == null)
                {
                    throw new ArgumentNullException(nameof(holderDTO), "Error. Request body was null");
                }

                var existingAccount = await _corporateRepo.GetValueAsync(e => e.CompanyCode == holderDTO.CompanyCode);

                if (existingAccount != null)
                {
                    throw new InvalidOperationException("Підприємство з даним кодом ЄДРПОУ вже існує в нашій базі.");
                }

                var relatedResponsiblePerson = await _corporateRepo.GetValueAsync(filter: r => r.ResponsiblePersonId == holderDTO.ResponsiblePersonId);

                if(relatedResponsiblePerson != null)
                {
                    throw new InvalidOperationException("Дана відповідальна особа вже прив'язана до іншої існуючої компанії");
                }

                CorporateAccountHolder accountHolder = new CorporateAccountHolder()
                {
                    ShortCompanyName = holderDTO.ShortCompanyName,
                    FullCompanyName = holderDTO.FullCompanyName,
                    CompanyCode = holderDTO.CompanyCode,
                    ContactEmail = holderDTO.ContactEmail,
                    ContactPhone = holderDTO.ContactPhone,
                    EmailConfirmed = holderDTO.EmailConfirmed,
                    DateOfEstablishment = DateTime.ParseExact(holderDTO.DateOfEstablishment, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    PostalCode = holderDTO.PostalCode,
                    Country = holderDTO.Country,
                    State = holderDTO.State,
                    City = holderDTO.City,
                    StreetAddress = holderDTO.StreetAddress,
                    IsActive = holderDTO.IsActive,
                    Status = holderDTO.Status,
                    ResponsiblePersonId = holderDTO.ResponsiblePersonId
                };

                await _corporateRepo.CreateAsync(accountHolder);
                await _corporateRepo.SaveChanges();

                _response.Result = accountHolder;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return Ok(_response);
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


        [HttpPut("UpdateCorporateAccountHolder/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateCorporateAccountHolder(int id, [FromForm] CorporateAccountHolderUpdateDTO holderDTO)
        {
            try
            {
                if (holderDTO == null)
                {
                    throw new ArgumentNullException(nameof(holderDTO), "Error. Request body was null");
                }

                if (id == 0 || id != holderDTO.AccountId)
                {
                    throw new ArgumentException("Error. Id of request and id of account don't correspond.");
                }

                var existingAccount = await _corporateRepo.GetValueAsync(filter: m => m.AccountId == id, isTracked: false);

                if (existingAccount == null)
                {
                    throw new ArgumentException($"Error. Account with id of {id} doesn't exist");
                }

                CorporateAccountHolder accountHolder = new CorporateAccountHolder()
                {
                    AccountId = holderDTO.AccountId,
                    ShortCompanyName = holderDTO.ShortCompanyName,
                    FullCompanyName = holderDTO.FullCompanyName,
                    CompanyCode = holderDTO.CompanyCode,
                    ContactEmail = holderDTO.ContactEmail,
                    ContactPhone = holderDTO.ContactPhone,
                    EmailConfirmed = holderDTO.EmailConfirmed,
                    DateOfEstablishment = DateTime.ParseExact(holderDTO.DateOfEstablishment, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    PostalCode = holderDTO.PostalCode,
                    Country = holderDTO.Country,
                    State = holderDTO.State,
                    City = holderDTO.City,
                    StreetAddress = holderDTO.StreetAddress,
                    IsActive = holderDTO.IsActive,
                    Status = holderDTO.Status,
                    ResponsiblePersonId = existingAccount.ResponsiblePersonId
                };


                await _corporateRepo.UpdateAccount(accountHolder);
                await _corporateRepo.SaveChanges();

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

        [HttpPatch("UpdateCorporatelAccountHolderPartially/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateCorporateAccountHolderPartially(int id, [FromBody] JsonPatchDocument<CorporateAccountHolderUpdateDTO> patchDTO)
        {
            try
            {

                var existingAccount = await _corporateRepo.GetValueAsync(filter: m => m.AccountId == id, isTracked: false);

                if (existingAccount == null)
                {
                    throw new ArgumentException($"Error. Account with id of {id} doesn't exist");
                }

                CorporateAccountHolderUpdateDTO holderDTO = new CorporateAccountHolderUpdateDTO()
                {
                    AccountId = existingAccount.AccountId,
                    ShortCompanyName = existingAccount.ShortCompanyName,
                    FullCompanyName = existingAccount.FullCompanyName,
                    CompanyCode = existingAccount.CompanyCode,
                    ContactEmail = existingAccount.ContactEmail,
                    ContactPhone = existingAccount.ContactPhone,
                    EmailConfirmed = existingAccount.EmailConfirmed,
                    DateOfEstablishment = existingAccount.DateOfEstablishment.ToLongDateString(),
                    PostalCode = existingAccount.PostalCode,
                    Country = existingAccount.Country,
                    State = existingAccount.State,
                    City = existingAccount.City,
                    StreetAddress = existingAccount.StreetAddress,
                    IsActive = existingAccount.IsActive,
                    Status = existingAccount.Status,
                };

                patchDTO.ApplyTo(holderDTO);

                CorporateAccountHolder accountHolder = new CorporateAccountHolder()
                {
                    AccountId = holderDTO.AccountId,
                    ShortCompanyName = holderDTO.ShortCompanyName,
                    FullCompanyName = holderDTO.FullCompanyName,
                    CompanyCode = holderDTO.CompanyCode,
                    ContactEmail = holderDTO.ContactEmail,
                    ContactPhone = holderDTO.ContactPhone,
                    EmailConfirmed = holderDTO.EmailConfirmed,
                    DateOfEstablishment = DateTime.ParseExact(holderDTO.DateOfEstablishment, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    PostalCode = holderDTO.PostalCode,
                    Country = holderDTO.Country,
                    State = holderDTO.State,
                    City = holderDTO.City,
                    StreetAddress = holderDTO.StreetAddress,
                    IsActive = holderDTO.IsActive,
                    Status = holderDTO.Status,
                    ResponsiblePersonId = existingAccount.ResponsiblePersonId
                };


                await _corporateRepo.UpdateAccount(accountHolder);
                await _corporateRepo.SaveChanges();

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
