using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO.TransactionDTOs;
using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Repository;
using PayBridgeAPI.Repository.TransactionRepo;
using Stripe;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IUserToUserTransactionRepository _userToUserRepo;
        private readonly IPersonalBankAccountRepository _personalAccountRepo;
        private readonly IBankCardRepository _bankCardRepo;
        private readonly IConfiguration _configuration;
        protected APIResponse _response;

        public TransactionController(IUserToUserTransactionRepository userToUserRepo, IPersonalBankAccountRepository personalAccountRepo, IBankCardRepository bankCardRepo, IConfiguration configuration)
        {
            _userToUserRepo = userToUserRepo;
            _personalAccountRepo = personalAccountRepo;
            _bankCardRepo = bankCardRepo;
            _response = new APIResponse();
            _configuration = configuration;
        }

        [HttpGet("GetUserToUserTransactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllUserToUserTransactions()
        {
            try
            {
                var transactionsQuery = await _userToUserRepo.GetAllTransactions(
                    include:
                    t => t
                    .Include(t => t.SenderBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.SenderBankCard.Account.Bank)
                    .Include(t => t.ReceiverBankCard).ThenInclude(t => t.Account).ThenInclude(t => t.AccountOwner).Include(t => t.ReceiverBankCard.Account.Bank));


                if (transactionsQuery.Count == 0)
                {
                    throw new NullReferenceException("Error. No user to user transaction were found in database by your request");
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

        [HttpGet("GetUserToUserTransaction/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUserToUserTransaction(int id)
        {
            try
            {
                var transactionQuery = await _userToUserRepo.GetSingleTransaction(
                    predicate:
                    t => t.TransactionId == id,
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


        [HttpPost("MakeTransaction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> MakePayment([FromBody]UserToUserTransactionCreateDTO transactionDTO)
        {
            try
            {
                if(transactionDTO == null)
                {
                    throw new NullReferenceException("Error. Your request body was null.");
                }

                var senderAccount = await _bankCardRepo.GetValueAsync(
                    filter: a => a.CardNumber == transactionDTO.SenderBankCardNumber,
                    include: a => a.Include(a => a.Account).ThenInclude(a => a.AccountOwner)
                    );

                var receiverAccount = await _bankCardRepo.GetValueAsync(
                    filter: a => a.CardNumber == transactionDTO.ReceiverBankCardNumber,
                    include: a => a.Include(a => a.Account).ThenInclude(a => a.AccountOwner)
                    );

                if(senderAccount == null || receiverAccount == null )
                {
                    throw new NullReferenceException("Error. Receiver or sender account wasn't found by your request. Please, check bank card info.");
                }

                if((senderAccount.Balance - transactionDTO.Amount) <= 0)
                {
                    throw new InvalidOperationException("Error. Sender account balance cannot afford transaction ammount");
                }

                long? amount = null;

                switch (transactionDTO.CurrencyCode)
                {
                    case "uah":
                        amount = (int)decimal.Floor(transactionDTO.Amount / 38.80m) * 100;
                        break;
                    case "usd":
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
                    Description = $"Переказ від" +
                    $" {receiverAccount.Account.AccountOwner.LastName}" +
                    $" {receiverAccount.Account.AccountOwner.FirstName[0]}." +
                    $"{receiverAccount.Account.AccountOwner.MiddleName[0]}." +
                    $" за номером карти {transactionDTO.ReceiverBankCardNumber}",
                };

                PaymentIntentService service = new PaymentIntentService();
                PaymentIntent response = service.Create(options);

                UserToUserTransaction transaction = new UserToUserTransaction()
                {
                    CurrencyCode = response.Currency,
                    Amount = response.Amount / 100 * 38.80m,
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

                switch(transaction.CurrencyCode)
                {
                    case "uah":
                        senderAccount.Balance -= transaction.Amount / 100 * 38.80m;
                        receiverAccount.Balance += transaction.Amount / 100 * 38.80m;
                        break;
                    case "usd":
                        senderAccount.Balance -= transaction.Amount / 100 * 38.80m;
                        receiverAccount.Balance += transaction.Amount / 100 * 38.80m;
                        break;
                }


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
    }
}
