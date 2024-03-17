using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO.BankCardDTOs;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository;
using System.Globalization;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [ApiController]
    [Route("api/bankCard")]
    public class BankCardController : ControllerBase
    {
        private readonly IBankCardRepository _bankCardRepository;
        private readonly IPersonalBankAccountRepository _bankAccountRepository;
        protected APIResponse _response;

        public BankCardController(IBankCardRepository bankCardRepository, IPersonalBankAccountRepository bankAccountRepository)
        {
            _bankCardRepository = bankCardRepository;
            _response = new APIResponse();
            _bankAccountRepository = bankAccountRepository;
        }

        [HttpGet("GetBankCards")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBankCards()
        {
            try
            {
                var bankCardsQuery = await _bankCardRepository.GetAllValues(orderBy: b => b.OrderBy(b => b.RegistrationDate), include: b => b.Include(b => b.Account.AccountOwner));

                if (bankCardsQuery.Count == 0)
                {
                    throw new NullReferenceException("Error. No bank cards were found by your request");
                }

                List<BankCardDTO> bankCards = new();
                foreach (var bankCard in bankCardsQuery)
                {
                    bankCards.Add(new BankCardDTO
                    {
                        BankCardId = bankCard.BankCardId,
                        CardNumber = bankCard.CardNumber,
                        ExpiryDate = bankCard.ExpiryDate.Month < 10 ? $"0{bankCard.ExpiryDate.Month}/{bankCard.ExpiryDate.Year % 100}" : $"{bankCard.ExpiryDate.Month}/{bankCard.ExpiryDate.Year % 100}",
                        OwnerCredentials = $"{bankCard.Account.AccountOwner.LastName} {bankCard.Account.AccountOwner.FirstName[0]}.{bankCard.Account.AccountOwner.MiddleName[0]}.",
                        CurrencyType = bankCard.CurrencyType,
                        Balance = bankCard.Balance,
                        IsActive = bankCard.IsActive,
                        CVC = bankCard.CVC,
                        RegistrationDate = bankCard.RegistrationDate.ToLongDateString(),
                    });
                }

                _response.Result = bankCards;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            
            catch(NullReferenceException ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }

        }

        [HttpGet("GetBankCard/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBankCard(int id)
        {
            try
            {
                var bankCardQuery = await _bankCardRepository.GetValueAsync(filter: b => b.BankCardId == id,orderBy: b => b.OrderBy(b => b.RegistrationDate), include: b => b.Include(b => b.Account.AccountOwner));

                if (bankCardQuery == null)
                {
                    throw new NullReferenceException("Error. No bank cards were found by your request");
                }

                BankCardDTO bankCard = new BankCardDTO()
                {
                    BankCardId = bankCardQuery.BankCardId,
                    CardNumber = bankCardQuery.CardNumber,
                    ExpiryDate = bankCardQuery.ExpiryDate.Month < 10 ? $"0{bankCardQuery.ExpiryDate.Month}/{bankCardQuery.ExpiryDate.Year % 100}" : $"{bankCardQuery.ExpiryDate.Month}/{bankCardQuery.ExpiryDate.Year % 100}",
                    OwnerCredentials = $"{bankCardQuery.Account.AccountOwner.LastName} {bankCardQuery.Account.AccountOwner.FirstName[0]}.{bankCardQuery.Account.AccountOwner.MiddleName[0]}.",
                    CurrencyType = bankCardQuery.CurrencyType,
                    Balance = bankCardQuery.Balance,
                    IsActive = bankCardQuery.IsActive,
                    CVC = bankCardQuery.CVC,
                    RegistrationDate = bankCardQuery.RegistrationDate.ToLongDateString(),
                };

                _response.Result = bankCard;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
        }

        [HttpPost("AddBankCard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> AddBankCard([FromBody]BankCardCreateDTO bankCardDTO)
        {
            try
            {
                if (bankCardDTO == null)
                {
                    throw new ArgumentNullException(nameof(bankCardDTO), "Error. Request body was null");
                }

                if(await _bankCardRepository.GetValueAsync(b => b.CardNumber == bankCardDTO.CardNumber) != null)
                {
                    throw new InvalidOperationException($"Error. Bank card with number of {bankCardDTO.CardNumber} already exists");
                }

                var bankCardsInDb = await _bankCardRepository.GetAllValues(orderBy: b => b.OrderBy(b => b.BankCardId), isTracked: false);
                var newBankCardId = bankCardsInDb.Last().BankCardId;

                BankCard bankCard = new()
                {
                    BankCardId = newBankCardId + 1,
                    CardNumber = bankCardDTO.CardNumber,
                    ExpiryDate = DateTime.ParseExact(bankCardDTO.ExpiryDate, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    CVC = bankCardDTO.CVC,
                    CurrencyType = bankCardDTO.CurrencyType,
                    Balance = bankCardDTO.Balance,
                    IsActive = bankCardDTO.IsActive,
                    BankAccountId = bankCardDTO.BankAccountId,
                    RegistrationDate = DateTime.Now
                };

                await _bankCardRepository.CreateAsync(bankCard);
                await _bankCardRepository.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = $"Bank Card with number of {bankCard.CardNumber} was successfully created";

                return Ok(_response);
            }
            
            catch(Exception ex)
            {
                if(ex is ArgumentNullException || ex is InvalidOperationException)
                {
                    _response.ErrorMessages.Add(ex.Message);
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(ex.Message);
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost("AttachCardToBankAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> AttachCardToBankAccount(int bankAccountId, int bankCardId)
        {
            try
            {
                if (bankAccountId <= 0 || bankCardId <= 0)
                {
                    throw new ArgumentException("Error. Id cannot be less than or equal to 0.");
                }

                var bankAccount = await _bankAccountRepository.GetValueAsync(filter: b => b.AccountId == bankAccountId);
                var bankCard = await _bankCardRepository.GetValueAsync(filter: b => b.BankCardId == bankCardId);

                if(bankCard == null || bankAccount == null)
                {
                    throw new NullReferenceException("Error. Bank Account or Bank Card wasn't found by your request");
                }

                bankAccount.BankCards.Add(bankCard);
                await _bankAccountRepository.UpdateAccount(bankAccount);
                await _bankAccountRepository.SaveChanges();

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Bank card has been successfully attached to bank account";
                _response.IsSuccess = true;
                return Ok(_response);

            }
            catch(Exception ex)
            {
                if(ex is ArgumentException)
                {
                    _response.ErrorMessages.Add(ex.Message);
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }
                else if(ex is NullReferenceException)
                {
                    _response.ErrorMessages.Add(ex.Message);
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return NotFound(_response);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
