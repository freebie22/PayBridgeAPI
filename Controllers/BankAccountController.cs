using Microsoft.AspNetCore.Mvc;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository;
using System.Globalization;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [ApiController]
    [Route("api/bankAccount")]
    public class BankAccountController : ControllerBase
    {
        private readonly IPersonalBankAccountRepository _personalAccountRepository;
        protected APIResponse _response;

        public BankAccountController(IPersonalBankAccountRepository personalAccountRepository)
        {
            _personalAccountRepository = personalAccountRepository;
            _response = new APIResponse();
        }

        [HttpGet("GetPersonalBankAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPersonalBankAccounts()
        {
            try
            {
                var query = await _personalAccountRepository.GetAllValues(includeProperties: "AccountOwner, AccountManager, Bank");

                if (query.Count == 0)
                {
                    throw new NullReferenceException("Error. No bank accounts have been found in database");
                }

                List<PersonalBankAccountDTO> bankAccounts = new List<PersonalBankAccountDTO>();
                foreach (PersonalBankAccount bankAccount in query)
                {
                    bankAccounts.Add(new PersonalBankAccountDTO()
                    {
                        AccountId = bankAccount.AccountId,
                        AccountNumber = bankAccount.AccountNumber,
                        AccountOwnerFullName = $"{bankAccount.AccountOwner.LastName} " + $"{bankAccount.AccountOwner.FirstName} " + $"{bankAccount.AccountOwner.MiddleName}",
                        AccountType = bankAccount.AccountType,
                        RegistratedByManager = $"{bankAccount.AccountManager.LastName} " + $"{bankAccount.AccountManager.FirstName} " + $"{bankAccount.AccountManager.MiddleName}",
                        BankName = bankAccount.Bank.ShortBankName,
                        RegistrationDate = bankAccount.RegistrationDate.ToLongDateString(),
                    });
                }

                _response.Result = bankAccounts;
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

        [HttpGet("GetPersonalBankAccounts/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPersonalBankAccount(int id)
        {
            try
            {
                var bankAccount = await _personalAccountRepository.GetValueAsync(filter: b => b.AccountId == id, includeProperties: "AccountOwner, AccountManager, Bank");

                if (bankAccount == null)
                {
                    throw new NullReferenceException($"Error. No bank accounts have been found in database by id {id}");
                }

                PersonalBankAccountDTO bankAccountDTO = new PersonalBankAccountDTO()
                {
                    AccountId = bankAccount.AccountId,
                    AccountNumber = bankAccount.AccountNumber,
                    AccountOwnerFullName = $"{bankAccount.AccountOwner.LastName} " + $"{bankAccount.AccountOwner.FirstName} " + $"{bankAccount.AccountOwner.MiddleName}",
                    AccountType = bankAccount.AccountType,
                    RegistratedByManager = $"{bankAccount.AccountManager.LastName} " + $"{bankAccount.AccountManager.FirstName} " + $"{bankAccount.AccountManager.MiddleName}",
                    BankName = bankAccount.Bank.ShortBankName,
                    RegistrationDate = bankAccount.RegistrationDate.ToLongDateString(),
                };

                _response.Result = bankAccountDTO;
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

        [HttpPost("RegisterPersonalBankAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> RegisterPersonalBankAccount([FromBody] PersonalBankAccountCreateDTO bankAccountDTO)
        {
            try
            {
                if (bankAccountDTO == null)
                {
                    throw new ArgumentNullException(nameof(bankAccountDTO), "Error. Request body was null");
                }


                PersonalBankAccount bankAccount = new PersonalBankAccount()
                {
                   AccountType = bankAccountDTO.AccountType,
                   IsActive = false,
                   Status = "Рахунок очікує на активацію",
                   AccountOwnerId = bankAccountDTO.AccountOwnerId,
                   ManagerId = bankAccountDTO.ManagerId,
                   BankId = bankAccountDTO.BankId,
                };


                await _personalAccountRepository.CreateAsync(bankAccount);
                await _personalAccountRepository.SaveChanges();

                _response.Result = bankAccount;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return CreatedAtRoute(nameof(GetPersonalBankAccount), new { id = bankAccount.AccountId }, _response);
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
