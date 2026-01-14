using Microsoft.AspNetCore.Mvc;
using CashFlow.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using CashFlow.Infrastructure.Data;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CashFlow.Api.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class RecTransactionsController : ControllerBase
    {
        private readonly IRecTransactionService _recTransactionService;

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);


        public RecTransactionsController(IRecTransactionService recTransactionService)
        {
            _recTransactionService = recTransactionService;
        }

        [HttpGet]
        [Route("rec-transaction-info")]
        public async Task<ActionResult<RecTransactionResponse>> GetAccountRecTransactions(int accountId)
        {
            var recTransactionsDto = await _recTransactionService.GetAccountRecTransactionsAsync(CurrentUserId, accountId);
            return Ok(recTransactionsDto);
        }

        [HttpPost]
        [Route("create-new-rec-transaction")]
        public async Task<IActionResult> CreateNewRecTransaction([FromBody] NewRecTransactionRequest request)
        {
            try
            {
                await _recTransactionService.CreateRecTransactionAsync(CurrentUserId, request);
                return Created();
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete-rec-transaction")]
        public async Task<IActionResult> DeleteRecTransaction(int transactionId, int accountId)
        {
            try
            {
                await _recTransactionService.DeleteRecTransactionAsync(CurrentUserId, transactionId, accountId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Rec transaction not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPatch]
        [Route("update-rec-transaction")]
        public async Task<IActionResult> UpdateRecTransaction([FromBody] UpdateRecTransactionRequest request)
        {
            try
            {
                await _recTransactionService.UpdateRecTransactionAsync(CurrentUserId, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Rec transaction not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPost("trigger-job")]
        [AllowAnonymous] // Opcjonalnie, ¿ebyœ nie musia³ siê logowaæ do testów joba
        public async Task<IActionResult> TriggerJob()
        {
            await _recTransactionService.ProcessPendingTransactionsAsync();
            return Ok("Job executed manually.");
        }
    }
}