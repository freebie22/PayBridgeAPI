using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO.TransactionDTOs;
using PayBridgeAPI.Repository.TransactionRepo;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IUserToUserTransactionRepository _userToUserRepo;
        protected APIResponse _response;

        public TransactionController(IUserToUserTransactionRepository userToUserRepo)
        {
            _userToUserRepo = userToUserRepo;
            _response = new APIResponse();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllUserToUserTransactions()
        {
            try
            {
                var transactionsQuery = await _userToUserRepo.GetAllTransactions(
                    include:
                    t => t.Include(t => t.SenderBankCard.Account.Bank).Include(t => t.ReceiverBankCard.Account.AccountOwner));

                if(transactionsQuery.Count == 0)
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
    }
}
