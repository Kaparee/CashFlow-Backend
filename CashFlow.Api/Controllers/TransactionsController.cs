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
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        [Route("transactions-info")]
        public async Task<ActionResult<TransactionResponse>> GetAccountTransactions(int accountId)
        {
            var transactionDto = await _transactionService.GetAccountTransactionsAsync(CurrentUserId, accountId);
            return Ok(transactionDto);
        }

        [HttpPost]
        [Route("create-new-transaction")]
        public async Task<IActionResult> CreateNewTransaction([FromBody] NewTransactionRequest request)
        {
            try
            {
                await _transactionService.CreateNewTransactionAsync(CurrentUserId, request);
                return Created();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("must be greater than 0"))
                {
                    return Conflict(new { message = ex.Message });
                }
                if (ex.Message.Contains("is required to"))
                {
                    return Conflict(new { message = ex.Message });
                }
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("delete-transaction")]
        public async Task<IActionResult> DeleteTransaction(int transactionId, int accountId)
        {
            try
            {
                await _transactionService.DeleteTransactionAsync(CurrentUserId, transactionId, accountId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Transaction not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPatch]
        [Route("update-transaction")]
        public async Task<IActionResult> UpdateTransaction([FromBody] UpdateTransactionRequest request)
        {
            try
            {
                await _transactionService.UpdateTransactionAsync(CurrentUserId, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Transaction not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }
    }
}