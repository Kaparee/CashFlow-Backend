using CashFlow.Application.DTO.Responses;
using CashFlow.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/")]
[ApiController]
public class CurrencyController : ControllerBase
{
    private readonly ICurrencyFetcher _currencyFetcher;
    private readonly ICurrencyService _currencyService;

    public CurrencyController(ICurrencyFetcher currencyFetcher, ICurrencyService currencyService)
    {
        _currencyFetcher = currencyFetcher;
        _currencyService = currencyService;
    }

    [HttpGet]
    [Route("currencies-info")]
    public async Task<ActionResult<IEnumerable<CurrencyResponse>>> GetCurrencies()
    {
        var currencyDto = await _currencyService.GetAllCurrenciesAsync();
        return Ok(currencyDto);
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncCurrencies()
    {
        await _currencyService.SyncRatesAsync();
        return Ok(new { message = "Kursy walut zostały zaktualizowane :)" });
    }
}