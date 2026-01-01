using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;
using CashFlow.Application.Interfaces;
using CashFlow.Application.Services;
using CashFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CashFlow.Api.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        [Route("accounts-info")]
        public async Task<ActionResult<AccountResponse>> GetUserAccounts()
        {
            try
            {
                var accountDto = await _accountService.GetUserAccountsAsync(CurrentUserId);
                return Ok(accountDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("does not have"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPost]
        [Route("create-new-account")]
        public async Task<IActionResult> CreateNewAccount([FromBody] NewAccountRequest request)
        {
            try
            {
                await _accountService.CreateNewAccountAsync(CurrentUserId, request);
                return Created();
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Given account name is already created"))
                {
                    return Conflict(new { message = ex.Message });
                }
                return StatusCode(500, new { message = "An internal server error occured" });
            }
        }

        [HttpDelete]
        [Route("delete-account")]
        public async Task<IActionResult> DeleteAccount(int accountId)
        {
            try
            {
                await _accountService.DeleteAccountAsync(CurrentUserId, accountId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("Account not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPatch]
        [Route("update-account")]
        public async Task<IActionResult> UpdateAccount([FromBody] UpdateAccountRequest request)
        {
            try
            {
                await _accountService.UpdateAccountAsync(CurrentUserId, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Account not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }
    }
}