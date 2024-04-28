using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO.BankCardDTOs;
using PayBridgeAPI.Models.DTO.CompanyBankAssetDTOs;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository;
using PayBridgeAPI.Repository.CompanyBankAssetRepository;
using System.Globalization;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [ApiController]
    [Route("api/bankAccount")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankCardRepository _bankCardRepository;
        private readonly ICompanyBankAssetRepository _bankAssetRepository;
        protected APIResponse _response;

        public BankAccountController(IBankCardRepository bankCardRepository, ICompanyBankAssetRepository bankAssetRepository)
        {
            _bankCardRepository = bankCardRepository;
            _bankAssetRepository = bankAssetRepository;
            _response = new APIResponse();
        }

        [HttpGet("GetBankCards")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBankCards([FromQuery]int? accountHolderId)
        {
            try
            {

                IList<BankCard> bankCardsQuery = new List<BankCard>();

                if(accountHolderId != null || accountHolderId <= 0)
                {
                    bankCardsQuery = await _bankCardRepository.GetAllValues(filter: b => b.Account.AccountId == accountHolderId ,orderBy: b => b.OrderBy(b => b.RegistrationDate), include: b => b.Include(b => b.Account.AccountOwner).Include(b => b.Account.Bank));
                }

                else
                {
                    bankCardsQuery = await _bankCardRepository.GetAllValues(orderBy: b => b.OrderBy(b => b.RegistrationDate), include: b => b.Include(b => b.Account.AccountOwner).Include(b => b.Account.Bank));
                }

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
                        BankEmitent = bankCard.Account.Bank.ShortBankName,
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
                var bankCardQuery = await _bankCardRepository.GetValueAsync(filter: b => b.BankCardId == id,orderBy: b => b.OrderBy(b => b.RegistrationDate), include: b => b.Include(b => b.Account.AccountOwner).Include(b => b.Account.Bank));

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
                    BankEmitent = bankCardQuery.Account.Bank.ShortBankName,
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
                    CVC = bankCardDTO.CVC,
                    CurrencyType = bankCardDTO.CurrencyType,
                    Balance = bankCardDTO.Balance,
                    IsActive = bankCardDTO.IsActive,
                    BankAccountId = bankCardDTO.BankAccountId,
                    RegistrationDate = DateTime.Now
                };

                string[] bankCardExpiryDate = bankCardDTO.ExpiryDate.Split('/');
                int month = int.Parse(bankCardExpiryDate[0]);
                int year = int.Parse("20" + bankCardExpiryDate[1]);

                if(month < 0 || month > 12)
                {
                    throw new ArgumentException("Помилка. Ви ввели неправильний місяць. Зверніть увагу, що місяць має бути від 01 до 12");
                }

                if (year < DateTime.Now.Year || year > (DateTime.Now.Year +10))
                {
                    throw new ArgumentException("Помилка. Ви ввели неправильний рік. Зверніть увагу, що місяць має бути не менший за теперішній, але не більший чим на 10 років");
                }

                bankCard.ExpiryDate = new DateTime(year, month, 1);

                await _bankCardRepository.CreateAsync(bankCard);
                await _bankCardRepository.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = $"Bank Card with number of {bankCard.CardNumber} was successfully created";

                return Ok(_response);
            }
            
            catch(Exception ex)
            {
                if(ex is ArgumentNullException || ex is InvalidOperationException || ex is ArgumentException)
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

        [HttpGet("GetBankAssets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetBankAssets([FromQuery]int? responsiblePersonId = null)
        {
            try
            {
                IList<CompanyBankAsset> bankAssetsQuery = new List<CompanyBankAsset>();

                if(responsiblePersonId != null)
                {
                    bankAssetsQuery = await _bankAssetRepository.GetAllValues(
                    filter: 
                    a => a.CorporateAccount.AccountOwner.ResponsiblePersonId == responsiblePersonId,
                    include:
                    a => a.Include(a => a.CorporateAccount).ThenInclude(a => a.AccountOwner).Include(a => a.CorporateAccount).ThenInclude(a => a.Bank)
                    );
                }

                else
                {
                    bankAssetsQuery = await _bankAssetRepository.GetAllValues(
                    include:
                    a => a.Include(a => a.CorporateAccount).ThenInclude(a => a.AccountOwner).Include(a => a.CorporateAccount).ThenInclude(a => a.Bank)
                    );
                }

                if (bankAssetsQuery.Count == 0)
                {
                    throw new NullReferenceException("За Вашим запитом не знайдено жодного розрахункового рахунку.");
                }

                List<CompanyBankAssetDTO> bankAssets = new();

                foreach(var companyBankAsset in bankAssetsQuery)
                {
                    bankAssets.Add(new CompanyBankAssetDTO()
                    {
                        AssetId = companyBankAsset.AssetId,
                        AssetUniqueNumber = companyBankAsset.AssetUniqueNumber,
                        BankAccountUniqueNumber = companyBankAsset.CorporateAccount.AccountNumber,
                        IBAN_Number = companyBankAsset.IBAN_Number,
                        CurrencyType = companyBankAsset.CurrencyType,
                        Balance = companyBankAsset.Balance,
                        IsActive = companyBankAsset.IsActive,
                        Status = companyBankAsset.Status,
                        BankName = companyBankAsset.CorporateAccount.Bank.ShortBankName,
                        ShortCompanyOwnerName = companyBankAsset.CorporateAccount.AccountOwner.ShortCompanyName,
                        RegistrationDate = companyBankAsset.RegistrationDate.ToLongDateString(),
                    });
                }

                _response.Result = bankAssets;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            catch(NullReferenceException ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
        }

        [HttpGet("GetBankAsset/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetBankAsset(int id)
        {
            try
            {
                var bankAssetQuery = await _bankAssetRepository.GetValueAsync(
                    filter:
                    a => a.AssetId == id,
                    include:
                    a => a.Include(a => a.CorporateAccount).ThenInclude(a => a.AccountOwner).Include(a => a.CorporateAccount).ThenInclude(a => a.Bank)
                    );

                if (bankAssetQuery == null)
                {
                    throw new NullReferenceException("Error. No company bank assets were found by your request");
                }

                CompanyBankAssetDTO bankAsset = new()
                {
                    AssetId = bankAssetQuery.AssetId,
                    AssetUniqueNumber = bankAssetQuery.AssetUniqueNumber,
                    IBAN_Number = bankAssetQuery.IBAN_Number,
                    CurrencyType = bankAssetQuery.CurrencyType,
                    Balance = bankAssetQuery.Balance,
                    IsActive = bankAssetQuery.IsActive,
                    Status = bankAssetQuery.Status,
                    BankName = bankAssetQuery.CorporateAccount.Bank.ShortBankName,
                    ShortCompanyOwnerName = bankAssetQuery.CorporateAccount.AccountOwner.ShortCompanyName,
                    RegistrationDate = bankAssetQuery.RegistrationDate.ToLongDateString(),
                };

                _response.Result = bankAsset;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.ErrorMessages.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
        }

        [HttpPost("CreateBankAsset")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateBankAssset([FromBody]CompanyBankAssetCreateDTO bankAssetCreateDTO)
        {
            try
            {
                if(bankAssetCreateDTO == null)
                {
                    throw new NullReferenceException("Error. Request body is null or damaged. Please, check all body fields");
                }

                if(await _bankAssetRepository.GetValueAsync(a => a.IBAN_Number == bankAssetCreateDTO.IBAN_Number, isTracked: false) != null)
                {
                    throw new ArgumentException($"Error. Bank asset with IBAN Number of {bankAssetCreateDTO.IBAN_Number} already exists. Please, choose another IBAN Number to continue");
                }


                CompanyBankAsset bankAsset = new()
                {
                    IBAN_Number = bankAssetCreateDTO.IBAN_Number,
                    CurrencyType = "uah",
                    Balance = default,
                    IsActive = true,
                    Status = "Рахунок активний",
                    CorporateAccountId = bankAssetCreateDTO.CorporateAccountId,
                };

                await _bankAssetRepository.CreateAsync(bankAsset);
                await _bankAssetRepository.SaveChangesAsync();

                _response.Result = "Bank asset was successfully created";
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }

            catch(Exception ex)
            {
                if(ex is ArgumentException || ex is NullReferenceException)
                {
                    _response.ErrorMessages.Add(ex.Message);
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
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
