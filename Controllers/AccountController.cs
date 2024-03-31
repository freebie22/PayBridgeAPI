using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.DTO.BankCardDTOs;
using PayBridgeAPI.Models.DTO.CompanyBankAssetDTOs;
using PayBridgeAPI.Models.DTO.CorporateBankAccountDTOs;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository;
using System.Globalization;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IPersonalBankAccountRepository _personalAccountRepository;
        private readonly ICorporateBankAccountRepository _corporateBankAccountRepository;
        protected APIResponse _response;

        public AccountController(IPersonalBankAccountRepository personalAccountRepository, ICorporateBankAccountRepository corporateBankAccountRepository)
        {
            _personalAccountRepository = personalAccountRepository;
            _response = new APIResponse();
            _corporateBankAccountRepository = corporateBankAccountRepository;
        }

        [HttpGet("GetPersonalBankAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPersonalBankAccounts()
        {
            try
            {
                var query = await _personalAccountRepository.GetAllValues(
                    include:
                        q => q.Include(q => q.AccountOwner).Include(q => q.AccountManager).Include(q => q.Bank).Include(q => q.BankCards)
                    );

                if (query.Count == 0)
                {
                    throw new NullReferenceException("Error. No bank accounts have been found in database");
                }

                List<PersonalBankAccountDTO> bankAccounts = new List<PersonalBankAccountDTO>();

                foreach (PersonalBankAccount bankAccount in query)
                {
                    List<BankCardDTO> bankCards = new List<BankCardDTO>();
                    PersonalBankAccountDTO account = new PersonalBankAccountDTO()
                    {
                        AccountId = bankAccount.AccountId,
                        AccountNumber = bankAccount.AccountNumber,
                        AccountOwnerFullName = $"{bankAccount.AccountOwner.LastName} " + $"{bankAccount.AccountOwner.FirstName} " + $"{bankAccount.AccountOwner.MiddleName}",
                        AccountType = bankAccount.AccountType,
                        RegistratedByManager = $"{bankAccount.AccountManager.LastName} " + $"{bankAccount.AccountManager.FirstName} " + $"{bankAccount.AccountManager.MiddleName}",
                        BankName = bankAccount.Bank.ShortBankName,
                        RegistrationDate = bankAccount.RegistrationDate.ToLongDateString(),
                    };
                    foreach(var bankCard in bankAccount.BankCards)
                    {
                        BankCardDTO bankCardDTO = new BankCardDTO()
                        {
                            BankCardId = bankCard.BankCardId,
                            CardNumber = bankCard.CardNumber,
                            ExpiryDate = bankCard.ExpiryDate.ToLongDateString(),
                            CVC = bankCard.CVC,
                            OwnerCredentials = $"{bankCard.Account.AccountOwner.LastName} {bankCard.Account.AccountOwner.FirstName[0]}.{bankCard.Account.AccountOwner.MiddleName[0]}.",
                            CurrencyType = bankCard.CurrencyType,
                            Balance = bankCard.Balance,
                            IsActive = bankCard.IsActive,
                            RegistrationDate = bankCard.RegistrationDate.ToLongDateString(),
                        };
                        bankCards.Add(bankCardDTO);
                    }
                    account.BankCards = bankCards;
                    bankAccounts.Add(account);
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

        [HttpGet("GetPersonalBankAccount/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetPersonalBankAccount(int id)
        {
            try
            {
                var bankAccount = await _personalAccountRepository.GetValueAsync(filter: b => b.AccountId == id, include:
                        q => q.Include(q => q.AccountOwner).Include(q => q.AccountManager).Include(q => q.Bank).Include(q => q.BankCards));

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

                List<BankCardDTO> bankCards = new();

                foreach(var bankCard in bankAccount.BankCards)
                {
                    BankCardDTO bankCardDTO = new BankCardDTO()
                    {
                        BankCardId = bankCard.BankCardId,
                        CardNumber = bankCard.CardNumber,
                        ExpiryDate = bankCard.ExpiryDate.ToLongDateString(),
                        CVC = bankCard.CVC,
                        OwnerCredentials = $"{bankCard.Account.AccountOwner.LastName} {bankCard.Account.AccountOwner.FirstName[0]}.{bankCard.Account.AccountOwner.MiddleName[0]}.",
                        CurrencyType = bankCard.CurrencyType,
                        Balance = bankCard.Balance,
                        IsActive = bankCard.IsActive,
                        RegistrationDate = bankCard.RegistrationDate.ToLongDateString(),
                    };
                    bankCards.Add(bankCardDTO);
                }

                bankAccountDTO.BankCards = bankCards;

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

        [HttpGet("GetCorporateBankAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCorporateBankAccounts()
        {
            try
            {
                var query = await _corporateBankAccountRepository.GetAllValues(
                    include:
                        q => q.Include(q => q.AccountOwner).Include(q => q.AccountOwner).Include(q => q.Bank).Include(q => q.Manager).Include(q => q.BankAssets)
                    );

                if (query.Count == 0)
                {
                    throw new NullReferenceException("Error. No bank accounts have been found in database");
                }

                List<CorporateBankAccountDTO> bankAccounts = new List<CorporateBankAccountDTO>();

                foreach (CorporateBankAccount bankAccount in query)
                {
                    List<CompanyBankAssetDTO> bankAssets = new List<CompanyBankAssetDTO>();
                    CorporateBankAccountDTO account = new CorporateBankAccountDTO()
                    {
                        AccountId = bankAccount.AccountId,
                        AccountNumber = bankAccount.AccountNumber,
                        CurrencyType = bankAccount.CurrencyType,
                        CompanyOwnerShortName = bankAccount.AccountOwner.ShortCompanyName,
                        CompanyCode = bankAccount.AccountOwner.CompanyCode,
                        AccountType = bankAccount.AccountType,
                        RegisteredByManager = $"{bankAccount.Manager.LastName} " + $"{bankAccount.Manager.FirstName} " + $"{bankAccount.Manager.MiddleName}",
                        BankName = bankAccount.Bank.ShortBankName,
                        RegistrationDate = bankAccount.RegistrationDate.ToLongDateString(),
                    };
                   foreach(var bankAsset in bankAccount.BankAssets)
                   {
                        bankAssets.Add(new CompanyBankAssetDTO()
                        {
                            AssetId = bankAsset.AssetId,
                            AssetUniqueNumber = bankAsset.AssetUniqueNumber,
                            BankAccountUniqueNumber = bankAsset.CorporateAccount.AccountNumber,
                            IBAN_Number = bankAsset.IBAN_Number,
                            CurrencyType = bankAsset.CurrencyType,
                            Balance = bankAsset.Balance,
                            IsActive = bankAsset.IsActive,
                            Status = bankAsset.Status,
                            BankName = bankAsset.CorporateAccount.Bank.ShortBankName,
                            ShortCompanyOwnerName = bankAsset.CorporateAccount.AccountOwner.ShortCompanyName,
                            RegistrationDate = bankAsset.RegistrationDate.ToLongDateString(),
                        });
                    }
                    account.BankAssets = bankAssets;
                    bankAccounts.Add(account);
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

        [HttpGet("GetCorporateBankAccount/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCorporateBankAccount(int id)
        {
            try
            {
                var bankAccount = await _corporateBankAccountRepository.GetValueAsync(
                    filter:
                    q => q.AccountId == id,
                    include:
                        q => q.Include(q => q.AccountOwner).Include(q => q.AccountOwner).Include(q => q.Bank).Include(q => q.Manager)
                    );

                if (bankAccount == null)
                {
                    throw new NullReferenceException($"Error. No bank accounts have been found in database by id {id}");
                }

                CorporateBankAccountDTO bankAccountDTO = new CorporateBankAccountDTO()
                {
                    AccountId = bankAccount.AccountId,
                    AccountNumber = bankAccount.AccountNumber,
                    CurrencyType = bankAccount.CurrencyType,
                    CompanyOwnerShortName = bankAccount.AccountOwner.ShortCompanyName,
                    CompanyCode = bankAccount.AccountOwner.CompanyCode,
                    AccountType = bankAccount.AccountType,
                    RegisteredByManager = $"{bankAccount.Manager.LastName} " + $"{bankAccount.Manager.FirstName} " + $"{bankAccount.Manager.MiddleName}",
                    BankName = bankAccount.Bank.ShortBankName,
                    RegistrationDate = bankAccount.RegistrationDate.ToLongDateString(),
                };

                List<CompanyBankAssetDTO> bankAssets = new();

                foreach (var bankAsset in bankAccount.BankAssets)
                {
                    bankAssets.Add(new CompanyBankAssetDTO()
                    {
                        AssetId = bankAsset.AssetId,
                        AssetUniqueNumber = bankAsset.AssetUniqueNumber,
                        BankAccountUniqueNumber = bankAsset.CorporateAccount.AccountNumber,
                        IBAN_Number = bankAsset.IBAN_Number,
                        CurrencyType = bankAsset.CurrencyType,
                        Balance = bankAsset.Balance,
                        IsActive = bankAsset.IsActive,
                        Status = bankAsset.Status,
                        BankName = bankAsset.CorporateAccount.Bank.ShortBankName,
                        ShortCompanyOwnerName = bankAsset.CorporateAccount.AccountOwner.ShortCompanyName,
                        RegistrationDate = bankAsset.RegistrationDate.ToLongDateString(),
                    });
                }

                bankAccountDTO.BankAssets = bankAssets;

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

        [HttpPost("RegisterCorporateBankAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> RegisterCorporateBankAccount([FromBody] CorporateBankAccountCreateDTO bankAccountDTO)
        {
            try
            {
                if (bankAccountDTO == null)
                {
                    throw new ArgumentNullException(nameof(bankAccountDTO), "Error. Request body was null");
                }


                CorporateBankAccount bankAccount = new CorporateBankAccount()
                {
                    AccountType = bankAccountDTO.AccountType,
                    CurrencyType = bankAccountDTO.CurrencyType,
                    IsActive = false,
                    Status = "Очікує на активацію менеджером платіжної системи",
                    AccountOwnerId = bankAccountDTO.CompanyOwnerId,
                    ManagerId = bankAccountDTO.ManagerId,
                    BankId = bankAccountDTO.BankId,
                };


                await _corporateBankAccountRepository.CreateAsync(bankAccount);
                await _corporateBankAccountRepository.SaveChanges();

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
