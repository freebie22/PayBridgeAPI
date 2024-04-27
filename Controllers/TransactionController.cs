using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.Currency;
using PayBridgeAPI.Models.DTO.TransactionDTOs;
using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository;
using PayBridgeAPI.Repository.TransactionRepo;
using PayBridgeAPI.Services.RESTServices;
using Stripe;
using System.Net;
using PayBridgeAPI.Utility;
using PayBridgeAPI.Repository.CompanyBankAssetRepository;
using PayBridgeAPI.Models.MainModels;

namespace PayBridgeAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IUserToUserTransactionRepository _userToUserRepo;
        private readonly IUserToCompanyTransactionRepository _userToCompanyRepo;
        private readonly ICompanyToUserTransactionRepository _companyToUserRepo;
        private readonly ICompanyToCompanyTransactionRepository _companyToCompanyRepo;
        private readonly IBankCardRepository _bankCardRepo;
        private readonly ICompanyBankAssetRepository _companyBankAssetRepo;
        private readonly IConfiguration _configuration;
        private readonly IBaseService _baseService;
        protected APIResponse _response;

        public TransactionController(IUserToUserTransactionRepository userToUserRepo,
            IUserToCompanyTransactionRepository userToCompanyRepo,
            ICompanyToUserTransactionRepository companyToUserRepo,
            ICompanyToCompanyTransactionRepository companyToCompanyRepo,
            IBaseService baseService, IBankCardRepository bankCardRepo, IConfiguration configuration, ICompanyBankAssetRepository companyBankAssetRepo)
        {
            _userToUserRepo = userToUserRepo;
            _userToCompanyRepo = userToCompanyRepo;
            _companyToUserRepo = companyToUserRepo;
            _companyToCompanyRepo = companyToCompanyRepo;
            _bankCardRepo = bankCardRepo;
            _response = new APIResponse();
            _configuration = configuration;
            _baseService = baseService;
            _companyBankAssetRepo = companyBankAssetRepo;
        }

        [HttpGet("GetAllUserToUserTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllUserToUserTransactions(
            [FromQuery(Name = "currencyCode")]string currencyCode = "",
            [FromQuery(Name = "transactionNumber")]string transactionNumber = "",
            [FromQuery(Name = "accountId")]int? accountId = null)
        {
            try
            {
                List<UserToUserTransaction> transactionsQuery;

                if (!string.IsNullOrEmpty(currencyCode))
                {
                    transactionsQuery = await _userToUserRepo.GetAllTransactions(
                    predicate:
                    t => t.CurrencyCode == currencyCode,
                    orderBy: 
                    t => t.OrderByDescending(t => t.DateOfTransaction),
                    include:
                    t => t
                    .Include(t => t.SenderBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankCard.Account.Bank)
                    .Include(t => t.ReceiverBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankCard.Account.Bank));
                }


                if (!string.IsNullOrEmpty(transactionNumber))
                {
                    transactionsQuery = await _userToUserRepo.GetAllTransactions(
                    predicate:
                    t => t.TransactionNumber == transactionNumber,
                    orderBy:
                    t => t.OrderByDescending(t => t.DateOfTransaction),
                    include:
                    t => t
                    .Include(t => t.SenderBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankCard.Account.Bank)
                    .Include(t => t.ReceiverBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankCard.Account.Bank));
                }

                if (accountId != null && accountId > 0)
                {
                    transactionsQuery = await _userToUserRepo.GetAllTransactions(
                    predicate:
                    t => t.Sender.AccountOwner.AccountId == accountId || t.Receiver.AccountOwner.AccountId == accountId,
                    orderBy:
                    t => t.OrderByDescending(t => t.DateOfTransaction),
                    include:
                    t => t
                    .Include(t => t.SenderBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankCard.Account.Bank)
                    .Include(t => t.ReceiverBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankCard.Account.Bank));
                }

                else
                {
                   transactionsQuery = await _userToUserRepo.GetAllTransactions(
                   orderBy:
                   t => t.OrderByDescending(t => t.DateOfTransaction),
                   include:
                   t => t
                   .Include(t => t.SenderBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankCard.Account.Bank)
                   .Include(t => t.ReceiverBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankCard.Account.Bank));
                }


                if (transactionsQuery.Count == 0)
                {
                    throw new NullReferenceException("За Вашим обліковим записом транзакцій не знайдено.");
                }

                List<UserToUserTransactionDTO> transactions = new();
                foreach (var transaction in transactionsQuery)
                {
                    transactions.Add(new UserToUserTransactionDTO()
                    {
                        TransactionId = transaction.TransactionId,
                        TransactionUniqueNumber = transaction.TransactionNumber,
                        SenderCredentials = $"{transaction.Sender.AccountOwner.LastName} {transaction.Sender.AccountOwner.FirstName[0]}.{transaction.Sender.AccountOwner.MiddleName[0]}",
                        SenderBankCardNumber = transaction.SenderBankCard.CardNumber,
                        SenderBankEmitent = transaction.Sender.Bank.ShortBankName,
                        ReceiverCredentials = $"{transaction.Receiver.AccountOwner.LastName} {transaction.Receiver.AccountOwner.FirstName[0]}.{transaction.Receiver.AccountOwner.MiddleName[0]}",
                        ReceiverBankCardNumber = transaction.ReceiverBankCard.CardNumber,
                        ReceiverBankEmitent = transaction.Receiver.Bank.ShortBankName,
                        SenderHolderId = transaction.Sender.AccountOwner.AccountId,
                        ReceiverHolderId = transaction.Receiver.AccountOwner.AccountId,
                        CurrencyCode = transaction.CurrencyCode,
                        Amount = transaction.Amount,
                        TransactionType = transaction.TransactionType,
                        DateOfTransaction = transaction.DateOfTransaction.ToLongDateString(),
                        Description = transaction.Description,
                        Fee = transaction.Fee ?? 0.0m,
                        Status = transaction.Status,
                        StripePaymentIntentID = transaction.StripePaymentIntentId
                    });
                }

                _response.Result = transactions;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }

        [HttpGet("GetUserToUserTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUserToUserTransaction([FromQuery]string transactionNumber = "")
        {
            try
            {
                var transactionQuery = await _userToUserRepo.GetSingleTransaction(
                    predicate:
                    t => t.TransactionNumber == transactionNumber,
                    include:
                    t => t
                    .Include(t => t.SenderBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankCard.Account.Bank)
                    .Include(t => t.ReceiverBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankCard.Account.Bank));

                if (transactionQuery == null)
                {
                    throw new NullReferenceException("Error. No user to user transaction were found in database by your request");
                }

                UserToUserTransactionDTO transaction = new()
                {
                    TransactionId = transactionQuery.TransactionId,
                    TransactionUniqueNumber = transactionQuery.TransactionNumber,
                    SenderCredentials = $"{transactionQuery.Sender.AccountOwner.LastName} {transactionQuery.Sender.AccountOwner.FirstName[0]}.{transactionQuery.Sender.AccountOwner.MiddleName[0]}",
                    SenderBankCardNumber = transactionQuery.SenderBankCard.CardNumber,
                    SenderBankEmitent = transactionQuery.Sender.Bank.ShortBankName,
                    ReceiverCredentials = $"{transactionQuery.Receiver.AccountOwner.LastName} {transactionQuery.Receiver.AccountOwner.FirstName[0]}.{transactionQuery.Receiver.AccountOwner.MiddleName[0]}",
                    ReceiverBankCardNumber = transactionQuery.ReceiverBankCard.CardNumber,
                    ReceiverBankEmitent = transactionQuery.Receiver.Bank.ShortBankName,
                    SenderHolderId = transactionQuery.Sender.AccountOwner.AccountId,
                    ReceiverHolderId = transactionQuery.Receiver.AccountOwner.AccountId,
                    CurrencyCode = transactionQuery.CurrencyCode,
                    Amount = transactionQuery.Amount,
                    TransactionType = transactionQuery.TransactionType,
                    DateOfTransaction = transactionQuery.DateOfTransaction.ToLongDateString(),
                    Description = transactionQuery.Description,
                    Fee = transactionQuery.Fee ?? 0.0m,
                    Status = transactionQuery.Status,
                    StripePaymentIntentID = transactionQuery.StripePaymentIntentId
                };

                _response.Result = transaction;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }

        private static string TransformDateTime(DateTime time)
        {
            int month, year;

            month = time.Month;

            year = time.Year % 100;

            string newTimeFormat = $"{month}/{year}";

            return newTimeFormat;
        }


        [HttpPost("MakeUserToUserTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> MakeUserToUserTransaction([FromBody]UserToUserTransactionCreateDTO transactionDTO)
        {
            try
            {
                if(transactionDTO == null)
                {
                    throw new NullReferenceException("Error. Your request body was null.");
                }

                BankCard senderAccount = new();


                if(transactionDTO.RechargingType == RechargingTypes.RechargingAnotherUserCard)
                {
                    string[] expiryDate = transactionDTO.SenderExpiryDate.Split('/');

                    int month = int.Parse(expiryDate[0]);
                    int year = int.Parse("20" + expiryDate[1]);

                    DateTime dateToSearch = new DateTime(year, month, 1);

                    senderAccount = await _bankCardRepo.GetValueAsync(
                    filter: a => a.CardNumber == transactionDTO.SenderBankCardNumber && a.ExpiryDate == dateToSearch && a.CVC == transactionDTO.SenderCVC,
                    include: a => a.Include(a => a.Account).ThenInclude(a => a.AccountOwner)
                    );
                }

                else if(transactionDTO.RechargingType == RechargingTypes.RechargingOwnCard)
                {
                    senderAccount = await _bankCardRepo.GetValueAsync(
                    filter: a => a.CardNumber == transactionDTO.SenderBankCardNumber,
                    include: a => a.Include(a => a.Account).ThenInclude(a => a.AccountOwner)
                    );
                }


                var receiverAccount = await _bankCardRepo.GetValueAsync(
                    filter: a => a.CardNumber == transactionDTO.ReceiverBankCardNumber,
                    include: a => a.Include(a => a.Account).ThenInclude(a => a.AccountOwner)
                    );

                if(senderAccount == null || receiverAccount == null )
                {
                    throw new NullReferenceException("Помилка. Отримувача або відправника за вказаною інформацією не знайдено. Будь-ласка, перевірте введені Вами дані.");
                }

                //var currencyResponse = await _baseService.SendAsync(new APIRequest()
                //{
                //    //Development URL
                //    RequestURL = "https://localhost:7112/api/currency/GetCurrencyRate",
                //    RequestType = API_Request_Type.GET
                //});

                //IEnumerable<CurrencyDTO> currency = JsonConvert.DeserializeObject<IEnumerable<CurrencyDTO>>(currencyResponse);

                if ((senderAccount.Balance - transactionDTO.Amount) <= 0)
                {
                    throw new InvalidOperationException("Помилка. На балансі даної карти немає такої суми.");
                }



                StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
                PaymentIntentCreateOptions options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)transactionDTO.Amount * 100,
                    Currency = "uah",
                    PaymentMethodTypes = new List<string>()
                    {
                        "card"
                    },
                    Description = $"Переказ від" +
                    $" {senderAccount.Account.AccountOwner.LastName}" +
                    $" {senderAccount.Account.AccountOwner.FirstName[0]}." +
                    $"{senderAccount.Account.AccountOwner.MiddleName[0]}." +
                    $" за номером карти {transactionDTO.ReceiverBankCardNumber}",
                };

                PaymentIntentService service = new PaymentIntentService();
                PaymentIntent response = service.Create(options);

                UserToUserTransaction transaction = new UserToUserTransaction()
                {
                    CurrencyCode = response.Currency,
                    Amount = response.Amount / 100,
                    TransactionType = "Переказ з банківської карти на іншу карту",
                    DateOfTransaction = response.Created.ToLocalTime(),
                    Description = response.Description,
                    Fee = response.ApplicationFeeAmount,
                    SenderId = senderAccount.Account.AccountId,
                    ReceiverId = receiverAccount.Account.AccountId,
                    SenderBankCardId = senderAccount.BankCardId,
                    ReceiverBankCardId = receiverAccount.BankCardId,
                    StripePaymentIntentId = response.Id,
                    Status = response.Status,
                };

                await _userToUserRepo.CreateTransaction(transaction);
                await _userToUserRepo.SaveChanges();

                senderAccount.Balance -= transaction.Amount;
                receiverAccount.Balance += transaction.Amount;

                await _bankCardRepo.UpdateRangeAsync(senderAccount, receiverAccount);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Переказ виконано успішно. Деталі транзакції Ви можете переглянути в особистому кабінеті";
                _response.IsSuccess = true;


                return Ok(_response);
            }

            catch(Exception ex)
            {
                if(ex is NullReferenceException || ex is InvalidOperationException)
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

        [HttpGet("GetAllUserToCompanyTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllUserToCompanyTransactions(
           [FromQuery(Name = "currencyCode")] string currencyCode = "",
           [FromQuery(Name = "transactionNumber")] string transactionNumber = "")
        {
            try
            {
                List<UserToCompanyTransaction> transactionsQuery;

                if (!string.IsNullOrEmpty(currencyCode))
                {
                    transactionsQuery = await _userToCompanyRepo.GetAllTransactions(
                    predicate:
                    t => t.CurrencyCode == currencyCode,
                    include:
                    t => t
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));
                }

                if (!string.IsNullOrEmpty(transactionNumber))
                {
                    transactionsQuery = await _userToCompanyRepo.GetAllTransactions(
                    predicate:
                    t => t.TransactionNumber == transactionNumber,
                    include:
                    t => t
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));
                }

                else
                {
                    transactionsQuery = await _userToCompanyRepo.GetAllTransactions(
                    include:
                    t => t
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));
                }


                if (transactionsQuery.Count == 0)
                {
                    throw new NullReferenceException("Error. No user to company transactions were found in database by your request");
                }

                List<UserToCompanyTransactionDTO> transactions = new();
                foreach (var transaction in transactionsQuery)
                {
                    transactions.Add(new UserToCompanyTransactionDTO()
                    {
                        TransactionId = transaction.TransactionId,
                        TransactionUniqueNumber = transaction.TransactionNumber,
                        SenderCredentials = $"{transaction.Sender.AccountOwner.LastName} {transaction.Sender.AccountOwner.FirstName[0]}.{transaction.Sender.AccountOwner.MiddleName[0]}",
                        SenderBankCardNumber = transaction.BankCard.CardNumber,
                        SenderBankEmitent = transaction.Sender.Bank.ShortBankName,
                        ReceiverCompanyShortName = transaction.CompanyReceiver.AccountOwner.ShortCompanyName,
                        Receiver_CBA_IBANNumber = transaction.ReceiverBankAsset.IBAN_Number,
                        ReceiverBankEmitent = transaction.CompanyReceiver.Bank.ShortBankName,
                        CurrencyCode = transaction.CurrencyCode,
                        Amount = transaction.Amount,
                        TransactionType = transaction.TransactionType,
                        DateOfTransaction = transaction.DateOfTransaction.ToLongDateString(),
                        Description = transaction.Description,
                        Fee = transaction.Fee ?? 0.0m,
                        Status = transaction.Status,
                        StripePaymentIntentID = transaction.StripePaymentIntentId
                    });
                }

                _response.Result = transactions;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }

        [HttpGet("GetUserToCompanyTransaction/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUserToCompanyTransaction(int id)
        {
            try
            {
                var transactionQuery = await _userToCompanyRepo.GetSingleTransaction(
                    predicate:
                    t => t.TransactionId == id,
                    include:
                    t => t
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));

                if (transactionQuery == null)
                {
                    throw new NullReferenceException("Error. No user to company transaction were found in database by your request");
                }

                UserToCompanyTransactionDTO transaction = new()
                {
                    TransactionId = transactionQuery.TransactionId,
                    TransactionUniqueNumber = transactionQuery.TransactionNumber,
                    SenderCredentials = $"{transactionQuery.Sender.AccountOwner.LastName} {transactionQuery.Sender.AccountOwner.FirstName[0]}.{transactionQuery.Sender.AccountOwner.MiddleName[0]}",
                    SenderBankCardNumber = transactionQuery.BankCard.CardNumber,
                    SenderBankEmitent = transactionQuery.Sender.Bank.ShortBankName,
                    ReceiverCompanyShortName = transactionQuery.CompanyReceiver.AccountOwner.ShortCompanyName,
                    Receiver_CBA_IBANNumber = transactionQuery.ReceiverBankAsset.IBAN_Number,
                    ReceiverBankEmitent = transactionQuery.CompanyReceiver.Bank.ShortBankName,
                    CurrencyCode = transactionQuery.CurrencyCode,
                    Amount = transactionQuery.Amount,
                    TransactionType = transactionQuery.TransactionType,
                    DateOfTransaction = transactionQuery.DateOfTransaction.ToLongDateString(),
                    Description = transactionQuery.Description,
                    Fee = transactionQuery.Fee ?? 0.0m,
                    Status = transactionQuery.Status,
                    StripePaymentIntentID = transactionQuery.StripePaymentIntentId
                };

                _response.Result = transaction;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }


        [HttpPost("MakeUserToCompanyTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> MakeUserToCompanyTransaction([FromBody] UserToCompanyTransactionCreateDTO transactionDTO)
        {
            try
            {
                if (transactionDTO == null)
                {
                    throw new NullReferenceException("Error. Your request body was null.");
                }

                var senderAccount = await _bankCardRepo.GetValueAsync(
                    filter: a => a.CardNumber == transactionDTO.SenderBankCardNumber,
                    include: a => a.Include(a => a.Account).ThenInclude(a => a.AccountOwner)
                    );

                var receiverAccount = await _companyBankAssetRepo.GetValueAsync(
                    filter: a => a.IBAN_Number == transactionDTO.ReceiverIBANNumber,
                    include: a => a.Include(a => a.CorporateAccount).ThenInclude(a => a.AccountOwner)
                    );

                if (senderAccount == null || receiverAccount == null)
                {
                    throw new NullReferenceException("Error. Receiver or sender account wasn't found by your request. Please, check bank card or IBAN Number info.");
                }

                if ((senderAccount.Balance - transactionDTO.Amount) <= 0)
                {
                    throw new InvalidOperationException("Error. Sender account balance cannot afford transaction ammount");
                }


                var currencyResponse = await _baseService.SendAsync(new APIRequest()
                {
                    //Development URL
                    RequestURL = "https://localhost:7112/api/currency/GetCurrencyInfo",
                    RequestType = API_Request_Type.GET
                });

                IEnumerable<CurrencyDTO> currency = JsonConvert.DeserializeObject<IEnumerable<CurrencyDTO>>(currencyResponse);

                long? amount = null;

                switch (transactionDTO.CurrencyCode.ToLower())
                {
                    case "uah":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                    case "usd":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                    case "eur":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                }

                StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
                PaymentIntentCreateOptions options = new PaymentIntentCreateOptions()
                {
                    Amount = amount,
                    Currency = transactionDTO.CurrencyCode.ToLower(),
                    PaymentMethodTypes = new List<string>()
                    {
                        "card"
                    },
                    Description = $"{transactionDTO.Description}",
                };

                PaymentIntentService service = new PaymentIntentService();
                PaymentIntent response = service.Create(options);

                UserToCompanyTransaction transaction = new UserToCompanyTransaction()
                {
                    CurrencyCode = response.Currency,
                    Amount = response.Amount / 100,
                    TransactionType = "Переказ з банківської карти на рахунок юридчної особи/ФОП",
                    DateOfTransaction = response.Created.ToLocalTime(),
                    Description = response.Description,
                    Fee = response.ApplicationFeeAmount,
                    SenderId = senderAccount.Account.AccountId,
                    CompanyReceiverId = receiverAccount.CorporateAccount.AccountId,
                    SenderBankCardId = senderAccount.BankCardId,
                    ReceiverBankAssetId = receiverAccount.AssetId,
                    StripePaymentIntentId = response.Id,
                    Status = response.Status,
                };

                await _userToCompanyRepo.CreateTransaction(transaction);
                await _userToCompanyRepo.SaveChanges();

                switch (transaction.CurrencyCode)
                {
                    case "uah":
                        senderAccount.Balance -= transaction.Amount;
                        receiverAccount.Balance += transaction.Amount;
                        break;
                    case "usd":
                        senderAccount.Balance -= transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "usd").Select(c => c.PriceBuy).FirstOrDefault();
                        receiverAccount.Balance += transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "usd").Select(c => c.PriceBuy).FirstOrDefault();
                        break;
                    case "eur":
                        senderAccount.Balance -= transaction.Amount * currency.Where(c => string.Equals(c.CurrencyCode.ToLower(), "eur")).Select(c => c.PriceBuy).FirstOrDefault();
                        receiverAccount.Balance += transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "eur").Select(c => c.PriceBuy).FirstOrDefault();
                        break;
                }


                await _bankCardRepo.UpdateAsync(senderAccount);
                await _companyBankAssetRepo.UpdateAsync(receiverAccount);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Переказ виконано успішно. Деталі транзакції Ви можете переглянути в особистому кабінеті";
                _response.IsSuccess = true;


                return Ok(_response);
            }

            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is InvalidOperationException)
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

        [HttpGet("GetAllCompanyToUserTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllCompanyToUserTransactions(
           [FromQuery(Name = "currencyCode")] string currencyCode = "",
           [FromQuery(Name = "transactionNumber")] string transactionNumber = "")
        {
            try
            {
                List<CompanyToUserTransaction> transactionsQuery;

                if (!string.IsNullOrEmpty(currencyCode))
                {
                    transactionsQuery = await _companyToUserRepo.GetAllTransactions(
                    predicate:
                    t => t.CurrencyCode == currencyCode,
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank));
                }

                if (!string.IsNullOrEmpty(transactionNumber))
                {
                    transactionsQuery = await _companyToUserRepo.GetAllTransactions(
                    predicate:
                    t => t.TransactionNumber == transactionNumber,
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank));
                }

                else
                {
                    transactionsQuery = await _companyToUserRepo.GetAllTransactions(
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank));
                }


                if (transactionsQuery.Count == 0)
                {
                    throw new NullReferenceException("Error. No company to user transactions were found in database by your request");
                }

                List<CompanyToUserTransactionDTO> transactions = new();
                foreach (var transaction in transactionsQuery)
                {
                    transactions.Add(new CompanyToUserTransactionDTO()
                    {
                        TransactionId = transaction.TransactionId,
                        TransactionUniqueNumber = transaction.TransactionNumber,
                        SenderCompanyShortName = transaction.CompanySender.AccountOwner.ShortCompanyName,
                        Sender_CBA_IBANNumber = transaction.SenderBankAsset.IBAN_Number,
                        SenderBankEmitent = transaction.CompanySender.Bank.ShortBankName,
                        ReceiverCredentials = $"{transaction.Receiver.AccountOwner.LastName} {transaction.Receiver.AccountOwner.FirstName[0]}.{transaction.Receiver.AccountOwner.MiddleName[0]}",
                        ReceiverBankCardNumber = transaction.BankCard.CardNumber,
                        ReceiverBankEmitent = transaction.Receiver.Bank.ShortBankName,
                        CurrencyCode = transaction.CurrencyCode,
                        Amount = transaction.Amount,
                        TransactionType = transaction.TransactionType,
                        DateOfTransaction = transaction.DateOfTransaction.ToLongDateString(),
                        Description = transaction.Description,
                        Fee = transaction.Fee ?? 0.0m,
                        Status = transaction.Status
                    });
                }

                _response.Result = transactions;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }

        [HttpGet("GetCompanyToUserTransaction/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCompanyToUserTransaction(int id)
        {
            try
            {
                var transactionQuery = await _companyToUserRepo.GetSingleTransaction(
                    predicate:
                    t => t.TransactionId == id,
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.BankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.BankCard.Account.Bank));

                if (transactionQuery == null)
                {
                    throw new NullReferenceException("Error. No company to user transaction were found in database by your request");
                }

                CompanyToUserTransactionDTO transaction = new()
                {
                    TransactionId = transactionQuery.TransactionId,
                    TransactionUniqueNumber = transactionQuery.TransactionNumber,
                    SenderCompanyShortName = transactionQuery.CompanySender.AccountOwner.ShortCompanyName,
                    Sender_CBA_IBANNumber = transactionQuery.SenderBankAsset.IBAN_Number,
                    SenderBankEmitent = transactionQuery.CompanySender.Bank.ShortBankName,
                    ReceiverCredentials = $"{transactionQuery.Receiver.AccountOwner.LastName} {transactionQuery.Receiver.AccountOwner.FirstName[0]}.{transactionQuery.Receiver.AccountOwner.MiddleName[0]}",
                    ReceiverBankCardNumber = transactionQuery.BankCard.CardNumber,
                    ReceiverBankEmitent = transactionQuery.Receiver.Bank.ShortBankName,
                    CurrencyCode = transactionQuery.CurrencyCode,
                    Amount = transactionQuery.Amount,
                    TransactionType = transactionQuery.TransactionType,
                    DateOfTransaction = transactionQuery.DateOfTransaction.ToLongDateString(),
                    Description = transactionQuery.Description,
                    Fee = transactionQuery.Fee ?? 0.0m,
                    Status = transactionQuery.Status
                };

                _response.Result = transaction;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }


        [HttpPost("MakeCompanyToUserTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> MakeCompanyToUserTransaction([FromBody] CompanyToUserTransactionCreateDTO transactionDTO)
        {
            try
            {
                if (transactionDTO == null)
                {
                    throw new NullReferenceException("Error. Your request body was null.");
                }

                var senderAccount = await _companyBankAssetRepo.GetValueAsync(
                    filter: a => a.IBAN_Number == transactionDTO.SenderIBANNumber,
                    include: a => a.Include(a => a.CorporateAccount).ThenInclude(a => a.AccountOwner)
                    );

                var receiverAccount = await _bankCardRepo.GetValueAsync(
                    filter: a => a.CardNumber == transactionDTO.ReceiverBankCardNumber,
                    include: a => a.Include(a => a.Account).ThenInclude(a => a.AccountOwner)
                    );

                if (senderAccount == null || receiverAccount == null)
                {
                    throw new NullReferenceException("Error. Receiver or sender account wasn't found by your request. Please, check bank card or IBAN Number info.");
                }

                if ((senderAccount.Balance - transactionDTO.Amount) <= 0)
                {
                    throw new InvalidOperationException("Error. Sender account balance cannot afford transaction ammount");
                }


                var currencyResponse = await _baseService.SendAsync(new APIRequest()
                {
                    //Development URL
                    RequestURL = "https://localhost:7112/api/currency/GetCurrencyInfo",
                    RequestType = API_Request_Type.GET
                });

                IEnumerable<CurrencyDTO> currency = JsonConvert.DeserializeObject<IEnumerable<CurrencyDTO>>(currencyResponse);

                long? amount = null;

                switch (transactionDTO.CurrencyCode.ToLower())
                {
                    case "uah":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                    case "usd":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                    case "eur":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                }

                CompanyToUserTransaction transaction = new CompanyToUserTransaction()
                {
                    CurrencyCode = transactionDTO.CurrencyCode,
                    Amount = (decimal)amount / 100,
                    TransactionType = "Переказ з рахунку юридчної особи/ФОП на банківську картку",
                    DateOfTransaction = DateTime.Now,
                    Description = transactionDTO.Description,
                    Fee = 0.0m,
                    CompanySenderId = senderAccount.CorporateAccount.AccountId,
                    ReceiverId = receiverAccount.Account.AccountId,
                    ReceiverBankCardId = receiverAccount.BankCardId,
                    SenderBankAssetId = senderAccount.AssetId,
                    Status = "Транзакція успішна",
                };

                await _companyToUserRepo.CreateTransaction(transaction);
                await _companyToUserRepo.SaveChanges();

                switch (transaction.CurrencyCode)
                {
                    case "uah":
                        senderAccount.Balance -= transaction.Amount;
                        receiverAccount.Balance += transaction.Amount;
                        break;
                    case "usd":
                        senderAccount.Balance -= transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "usd").Select(c => c.PriceBuy).FirstOrDefault();
                        receiverAccount.Balance += transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "usd").Select(c => c.PriceBuy).FirstOrDefault();
                        break;
                    case "eur":
                        senderAccount.Balance -= transaction.Amount * currency.Where(c => string.Equals(c.CurrencyCode.ToLower(), "eur")).Select(c => c.PriceBuy).FirstOrDefault();
                        receiverAccount.Balance += transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "eur").Select(c => c.PriceBuy).FirstOrDefault();
                        break;
                }


                await _bankCardRepo.UpdateAsync(receiverAccount);
                await _companyBankAssetRepo.UpdateAsync(senderAccount);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Переказ виконано успішно. Деталі транзакції Ви можете переглянути в особистому кабінеті";
                _response.IsSuccess = true;


                return Ok(_response);
            }

            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is InvalidOperationException)
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

        [HttpGet("GetAllCompanyToCompanyTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllCompanyToCompanyTransactions(
           [FromQuery(Name = "currencyCode")] string currencyCode = "",
           [FromQuery(Name = "transactionNumber")] string transactionNumber = "")
        {
            try
            {
                List<CompanyToCompanyTransaction> transactionsQuery;

                if (!string.IsNullOrEmpty(currencyCode))
                {
                    transactionsQuery = await _companyToCompanyRepo.GetAllTransactions(
                    predicate:
                    t => t.CurrencyCode == currencyCode,
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));
                }

                if (!string.IsNullOrEmpty(transactionNumber))
                {
                    transactionsQuery = await _companyToCompanyRepo.GetAllTransactions(
                    predicate:
                    t => t.TransactionNumber == transactionNumber,
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));
                }

                else
                {
                    transactionsQuery = await _companyToCompanyRepo.GetAllTransactions(
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));
                }


                if (transactionsQuery.Count == 0)
                {
                    throw new NullReferenceException("Error. No company to company transactions were found in database by your request");
                }

                List<CompanyToCompanyTransactionDTO> transactions = new();
                foreach (var transaction in transactionsQuery)
                {
                    transactions.Add(new CompanyToCompanyTransactionDTO()
                    {
                        TransactionId = transaction.TransactionId,
                        TransactionUniqueNumber = transaction.TransactionNumber,
                        SenderCompanyShortName = transaction.CompanySender.AccountOwner.ShortCompanyName,
                        Sender_CBA_IBANNumber = transaction.SenderBankAsset.IBAN_Number,
                        SenderBankEmitent = transaction.CompanySender.Bank.ShortBankName,
                        ReceiverCompanyShortName = transaction.CompanyReceiver.AccountOwner.ShortCompanyName,
                        Receiver_CBA_IBANNumber = transaction.ReceiverBankAsset.IBAN_Number,
                        ReceiverBankEmitent = transaction.CompanyReceiver.Bank.ShortBankName,
                        CurrencyCode = transaction.CurrencyCode,
                        Amount = transaction.Amount,
                        TransactionType = transaction.TransactionType,
                        DateOfTransaction = transaction.DateOfTransaction.ToLongDateString(),
                        Description = transaction.Description,
                        Fee = transaction.Fee ?? 0.0m,
                        Status = transaction.Status
                    });
                }

                _response.Result = transactions;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }

        [HttpGet("GetCompanyToCompanyTransaction/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetCompanyToCompanyTransaction(int id)
        {
            try
            {
                var transactionQuery = await _companyToCompanyRepo.GetSingleTransaction(
                    predicate:
                    t => t.TransactionId == id,
                    include:
                    t => t
                    .Include(t => t.SenderBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankAsset.CorporateAccount.Bank)
                    .Include(t => t.ReceiverBankAsset).ThenInclude(t => t.CorporateAccount).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankAsset.CorporateAccount.Bank));

                if (transactionQuery == null)
                {
                    throw new NullReferenceException("Error. No company to user transaction were found in database by your request");
                }

                CompanyToCompanyTransactionDTO transaction = new()
                {
                    TransactionId = transactionQuery.TransactionId,
                    TransactionUniqueNumber = transactionQuery.TransactionNumber,
                    SenderCompanyShortName = transactionQuery.CompanySender.AccountOwner.ShortCompanyName,
                    Sender_CBA_IBANNumber = transactionQuery.SenderBankAsset.IBAN_Number,
                    SenderBankEmitent = transactionQuery.CompanySender.Bank.ShortBankName,
                    ReceiverCompanyShortName = transactionQuery.CompanyReceiver.AccountOwner.ShortCompanyName,
                    Receiver_CBA_IBANNumber = transactionQuery.ReceiverBankAsset.IBAN_Number,
                    ReceiverBankEmitent = transactionQuery.CompanyReceiver.Bank.ShortBankName,
                    CurrencyCode = transactionQuery.CurrencyCode,
                    Amount = transactionQuery.Amount,
                    TransactionType = transactionQuery.TransactionType,
                    DateOfTransaction = transactionQuery.DateOfTransaction.ToLongDateString(),
                    Description = transactionQuery.Description,
                    Fee = transactionQuery.Fee ?? 0.0m,
                    Status = transactionQuery.Status
                };

                _response.Result = transaction;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;

                return Ok(_response);
            }

            catch (NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages.Add(ex.Message);
                return NotFound(_response);
            }
        }


        [HttpPost("MakeCompanyToCompanyTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> MakeCompanyToCompanyTransaction([FromBody] CompanyToCompanyTransactionCreateDTO transactionDTO)
        {
            try
            {
                if (transactionDTO == null)
                {
                    throw new NullReferenceException("Error. Your request body was null.");
                }

                var senderAccount = await _companyBankAssetRepo.GetValueAsync(
                    filter: a => a.IBAN_Number == transactionDTO.SenderIBANNumber,
                    include: a => a.Include(a => a.CorporateAccount).ThenInclude(a => a.AccountOwner)
                    );

                var receiverAccount = await _companyBankAssetRepo.GetValueAsync(
                    filter: a => a.IBAN_Number == transactionDTO.ReceiverIBANNumber,
                    include: a => a.Include(a => a.CorporateAccount).ThenInclude(a => a.AccountOwner)
                    );

                if (senderAccount == null || receiverAccount == null)
                {
                    throw new NullReferenceException("Error. Receiver or sender account wasn't found by your request. Please, check bank card or IBAN Number info.");
                }

                if ((senderAccount.Balance - transactionDTO.Amount) <= 0)
                {
                    throw new InvalidOperationException("Error. Sender account balance cannot afford transaction ammount");
                }


                var currencyResponse = await _baseService.SendAsync(new APIRequest()
                {
                    //Development URL
                    RequestURL = "https://localhost:7112/api/currency/GetCurrencyInfo",
                    RequestType = API_Request_Type.GET
                });

                IEnumerable<CurrencyDTO> currency = JsonConvert.DeserializeObject<IEnumerable<CurrencyDTO>>(currencyResponse);

                long? amount = null;

                switch (transactionDTO.CurrencyCode.ToLower())
                {
                    case "uah":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                    case "usd":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                    case "eur":
                        amount = (int)transactionDTO.Amount * 100;
                        break;
                }

                CompanyToUserTransaction transaction = new CompanyToUserTransaction()
                {
                    CurrencyCode = transactionDTO.CurrencyCode,
                    Amount = (decimal)amount / 100,
                    TransactionType = "Переказ з рахунку юридчної особи/ФОП на рахунок іншої юридичної особи/ФОП",
                    DateOfTransaction = DateTime.Now,
                    Description = transactionDTO.Description,
                    Fee = 0.0m,
                    CompanySenderId = senderAccount.CorporateAccount.AccountId,
                    ReceiverId = receiverAccount.CorporateAccount.AccountId,
                    ReceiverBankCardId = receiverAccount.AssetId,
                    SenderBankAssetId = senderAccount.AssetId,
                    Status = "Транзакція успішна",
                };

                await _companyToUserRepo.CreateTransaction(transaction);
                await _companyToUserRepo.SaveChanges();

                switch (transaction.CurrencyCode)
                {
                    case "uah":
                        senderAccount.Balance -= transaction.Amount;
                        receiverAccount.Balance += transaction.Amount;
                        break;
                    case "usd":
                        senderAccount.Balance -= transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "usd").Select(c => c.PriceBuy).FirstOrDefault();
                        receiverAccount.Balance += transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "usd").Select(c => c.PriceBuy).FirstOrDefault();
                        break;
                    case "eur":
                        senderAccount.Balance -= transaction.Amount * currency.Where(c => string.Equals(c.CurrencyCode.ToLower(), "eur")).Select(c => c.PriceBuy).FirstOrDefault();
                        receiverAccount.Balance += transaction.Amount * currency.Where(c => c.CurrencyCode.ToLower() == "eur").Select(c => c.PriceBuy).FirstOrDefault();
                        break;
                }


                await _companyBankAssetRepo.UpdateAsync(senderAccount);
                await _companyBankAssetRepo.UpdateAsync(receiverAccount);

                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = "Переказ виконано успішно. Деталі транзакції Ви можете переглянути в особистому кабінеті";
                _response.IsSuccess = true;


                return Ok(_response);
            }

            catch (Exception ex)
            {
                if (ex is NullReferenceException || ex is InvalidOperationException)
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