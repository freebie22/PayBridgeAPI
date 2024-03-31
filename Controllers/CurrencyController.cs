using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PayBridgeAPI.Models;
using PayBridgeAPI.Models.Currency;
using PayBridgeAPI.Services.RESTServices;
using PayBridgeAPI.Utility;
using System.Globalization;
using System.Net;

namespace PayBridgeAPI.Controllers
{
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        protected APIResponse _response;
        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
            _response = new APIResponse();
        }


        [HttpGet("GetCurrencyInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetCurrencyInfo()
        {
            try
            {
                var currencyResponse = await _currencyService.GetCurrencyInfo();

                if (currencyResponse != null && currencyResponse.Length > 0)
                {
                    var currency = JsonConvert.DeserializeObject<List<Currency>>(currencyResponse);
                    var currencyDTO = currency.GetCurrency();

                    _response.Result = currencyDTO;
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.OK;

                    Response.Headers.Append("PrivatBankAPI", $"Request was made at {DateTime.Now}");

                    return Ok(_response.Result);
                }

                else
                {
                    throw new NullReferenceException("Error. No currency exchange info was found by your request.");
                }
            }
            catch(NullReferenceException ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(ex.Message);
                return BadRequest(ex.Message);
            }
            
        }
    }
}
