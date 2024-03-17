using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.DTO;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository;
using System.Globalization;
using System.Net;
using System.Text;

namespace PayBridgeAPI.Controllers
{
    [Route("api/Bank")]
    [ApiController]
    public class BankController : ControllerBase
    {
        private readonly IBankRepository _repository;

        protected APIResponse _response;

        public BankController(IBankRepository repository)
        {
            _repository = repository;
            _response = new APIResponse();
        }

        [HttpGet("GetBanks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBanks()
        {
            try
            {
                var query = await _repository.GetAllValues();

                if (query.Count == 0)
                {
                    throw new NullReferenceException("Error. No banks have been found in database");
                }

                IList<BankDTO> banks = new List<BankDTO>();
                foreach (Bank bank in query)
                {
                    banks.Add(new BankDTO()
                    {
                        BankId = bank.BankId,
                        FullBankName = bank.FullBankName,
                        ShortBankName = bank.ShortBankName,
                        RegistraionNumber = bank.RegistraionNumber,
                        IBAN = bank.IBAN,
                        NationalStateRegistryNumber = bank.NationalStateRegistryNumber,
                        BankIdentificationCode = bank.BankIdentificationCode,
                        SettlementAccount = bank.SettlementAccount,
                        TaxIdentificationNumber = bank.TaxIdentificationNumber,
                        NationalBankLicense = bank.NationalBankLicense,
                        SWIFTCode = bank.SWIFTCode,
                        EstablishedDate = bank.EstablishedDate.Day < 10 ? 0 + bank.EstablishedDate.ToLongDateString() : bank.EstablishedDate.ToLongDateString(),
                        HeadquarterAddress = bank.HeadquarterAddress,
                        ContactNumber = bank.ContactNumber,
                        ContactEmail = bank.ContactEmail,
                        WebsiteURL = bank.WebsiteURL,             
                        IsActive = bank.IsActive,
                        Status = bank.Status,                       
                    });
                }

                _response.Result = banks;
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

        [HttpGet("GetBank/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetBank(int id)
        {
            try
            {
                var bank = await _repository.GetValueAsync(filter: b => b.BankId == id);

                if (bank == null)
                {
                    throw new NullReferenceException($"Error. No banks have been found in database by id {id}");
                }

                BankDTO bankDTO = new BankDTO()
                {
                    BankId = bank.BankId,
                    FullBankName = bank.FullBankName,
                    ShortBankName = bank.ShortBankName,
                    RegistraionNumber = bank.RegistraionNumber,
                    IBAN = bank.IBAN,
                    NationalStateRegistryNumber = bank.NationalStateRegistryNumber,
                    BankIdentificationCode = bank.BankIdentificationCode,
                    SettlementAccount = bank.SettlementAccount,
                    TaxIdentificationNumber = bank.TaxIdentificationNumber,
                    NationalBankLicense = bank.NationalBankLicense,
                    SWIFTCode = bank.SWIFTCode,
                    EstablishedDate = bank.EstablishedDate.Day < 10 ? 0 + bank.EstablishedDate.ToLongDateString() : bank.EstablishedDate.ToLongDateString(),
                    HeadquarterAddress = bank.HeadquarterAddress,
                    ContactNumber = bank.ContactNumber,
                    ContactEmail = bank.ContactEmail,
                    WebsiteURL = bank.WebsiteURL,
                    IsActive = bank.IsActive,
                    Status = bank.Status,
                };

                _response.Result = bankDTO;
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


        [HttpPost("RegisterBank")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> RegisterBank([FromBody] BankCreateDTO bankDTO)
        {
            try
            {
                if (bankDTO == null)
                {
                    throw new ArgumentNullException(nameof(bankDTO), "Error. Request body was null");
                }


                Bank bank = new Bank()
                {
                    FullBankName = bankDTO.FullBankName,
                    ShortBankName = bankDTO.ShortBankName,
                    IBAN = bankDTO.IBAN,
                    NationalStateRegistryNumber = bankDTO.NationalStateRegistryNumber,
                    BankIdentificationCode = bankDTO.BankIdentificationCode,
                    SettlementAccount = bankDTO.SettlementAccount,
                    TaxIdentificationNumber = bankDTO.TaxIdentificationNumber,
                    NationalBankLicense = bankDTO.NationalBankLicense,
                    SWIFTCode = bankDTO.SWIFTCode,
                    EstablishedDate = DateTime.ParseExact(bankDTO.EstablishedDate, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    HeadquarterAddress = bankDTO.HeadquarterAddress,
                    ContactNumber = bankDTO.ContactNumber,
                    ContactEmail = bankDTO.ContactEmail,
                    WebsiteURL = bankDTO.WebsiteURL,
                };

                bank.IsActive = false;
                bank.Status = "Банк очікує на активацію.";

                await _repository.CreateAsync(bank);
                await _repository.SaveChangesAsync();

                _response.Result = bank;
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;

                return CreatedAtRoute(nameof(GetBank), new { id = bank.BankId }, _response);
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

        [HttpPut("UpdateBank/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateBank(int id, [FromForm] BankUpdateDTO bankDTO)
        {
            try
            {
                if (bankDTO == null)
                {
                    throw new ArgumentNullException(nameof(bankDTO), "Error. Request body was null.");
                }

                if (id == 0 || id != bankDTO.BankId)
                {
                    throw new ArgumentException("Error. Id of request and id of bank don't correspond.");
                }

                var existingBank = await _repository.GetValueAsync(filter: m => m.BankId == id, isTracked: false);

                if (existingBank == null)
                {
                    throw new ArgumentException($"Error. Bank with id of {id} doesn't exist");
                }

                Bank bank = new Bank()
                {
                    BankId = bankDTO.BankId,
                    FullBankName = bankDTO.FullBankName,
                    ShortBankName = bankDTO.ShortBankName,
                    IBAN = bankDTO.IBAN,
                    NationalStateRegistryNumber = bankDTO.NationalStateRegistryNumber,
                    BankIdentificationCode = bankDTO.BankIdentificationCode,
                    SettlementAccount = bankDTO.SettlementAccount,
                    TaxIdentificationNumber = bankDTO.TaxIdentificationNumber,
                    NationalBankLicense = bankDTO.NationalBankLicense,
                    SWIFTCode = bankDTO.SWIFTCode,
                    EstablishedDate = DateTime.ParseExact(bankDTO.EstablishedDate, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    HeadquarterAddress = bankDTO.HeadquarterAddress,
                    ContactNumber = bankDTO.ContactNumber,
                    ContactEmail = bankDTO.ContactEmail,
                    WebsiteURL = bankDTO.WebsiteURL,
                    IsActive = bankDTO.IsActive,
                    Status = bankDTO.Status
                };


                await _repository.UpdateAsync(bank);
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

        [HttpPatch("UpdateBankPartially/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateBankPartially(int id, [FromBody] JsonPatchDocument<BankUpdateDTO> patchDTO)
        {
            try
            {

                var existingBank = await _repository.GetValueAsync(filter: m => m.BankId == id, isTracked: false);

                if (existingBank == null)
                {
                    throw new ArgumentException($"Error. Bank with id of {id} doesn't exist.");
                }

                BankUpdateDTO bankDTO = new BankUpdateDTO()
                {
                    BankId = existingBank.BankId,
                    FullBankName = existingBank.FullBankName,
                    ShortBankName = existingBank.ShortBankName,
                    IBAN = existingBank.IBAN,
                    NationalStateRegistryNumber = existingBank.NationalStateRegistryNumber,
                    BankIdentificationCode = existingBank.BankIdentificationCode,
                    SettlementAccount = existingBank.SettlementAccount,
                    TaxIdentificationNumber = existingBank.TaxIdentificationNumber,
                    NationalBankLicense = existingBank.NationalBankLicense,
                    SWIFTCode = existingBank.SWIFTCode,
                    EstablishedDate = existingBank.EstablishedDate.Day < 10 ? 0 + existingBank.EstablishedDate.ToLongDateString() : existingBank.EstablishedDate.ToLongDateString(),
                    HeadquarterAddress = existingBank.HeadquarterAddress,
                    ContactNumber = existingBank.ContactNumber,
                    ContactEmail = existingBank.ContactEmail,
                    WebsiteURL = existingBank.WebsiteURL,
                    IsActive = existingBank.IsActive,
                    Status = existingBank.Status
                };

                patchDTO.ApplyTo(bankDTO);

                Bank updatedBank = new Bank()
                {
                    BankId = bankDTO.BankId,
                    FullBankName = bankDTO.FullBankName,
                    ShortBankName = bankDTO.ShortBankName,
                    IBAN = bankDTO.IBAN,
                    NationalStateRegistryNumber = bankDTO.NationalStateRegistryNumber,
                    BankIdentificationCode = bankDTO.BankIdentificationCode,
                    SettlementAccount = bankDTO.SettlementAccount,
                    TaxIdentificationNumber = bankDTO.TaxIdentificationNumber,
                    NationalBankLicense = bankDTO.NationalBankLicense,
                    SWIFTCode = bankDTO.SWIFTCode,
                    EstablishedDate = DateTime.ParseExact(bankDTO.EstablishedDate, "dd MMMM yyyy 'р.'", CultureInfo.GetCultureInfo("uk-UA")),
                    HeadquarterAddress = bankDTO.HeadquarterAddress,
                    ContactNumber = bankDTO.ContactNumber,
                    ContactEmail = bankDTO.ContactEmail,
                    WebsiteURL = bankDTO.WebsiteURL,
                    IsActive = bankDTO.IsActive,
                    Status = bankDTO.Status
                };

                await _repository.UpdateAsync(updatedBank);
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
