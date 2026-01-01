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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
        {
            try
            {
                await _userService.RegisterAsync(request);
                return Created();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("email or nickname is taken"))
                {
                    return Conflict(new { message = ex.Message });
                }
                return StatusCode(500, new { message = "An internal server error occured" });
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> LoginUser([FromBody] LoginRequest request)
        {
            try
            {
                var token = await _userService.LoginAsync(request);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("login-info")]
        public async Task<ActionResult<UserResponse>> GetUser()
        {
            try
            {
                var userDto = await _userService.GetUserByIdAsync(CurrentUserId);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("User was not found"))
                {
                    return NotFound();
                }
                throw;
            }
            
        }

        [HttpGet]
        [Route("verify")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail([FromQuery] string verificationToken)
        {
            try
            {
                await _userService.VerifyEmailAsync(verificationToken);
                return Ok("Poprawnie zweryfikowano!");
            }
            catch(Exception ex)
            {
                if(ex.Message.Contains("Invalid token"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpDelete]
        [Route("delete-user")]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                await _userService.DeleteUserAsync(CurrentUserId);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("User not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPatch]
        [Route("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                await _userService.UpdateUserAsync(CurrentUserId, request);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("User not found"))
                {
                    return NotFound();
                }
                throw;
            }
        }

        [HttpPatch]
        [Route("modify-password")]
        public async Task<IActionResult> ModifyPassword([FromBody] ModifyPasswordRequest request)
        {
            try
            {
                await _userService.ModifyPasswordAsync(CurrentUserId, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] ForgotPasswordRequest request)
        {
            await _userService.RequestPasswordResetAsync(request.Email);
            return Ok(new { message = "If the email is in our system, a reset link has been sent." });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("confirm-password-reset")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _userService.ResetPasswordConfirmAsync(request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Route("request-email-change")]
        public async Task<IActionResult> RequestEmailChange([FromBody] RequestEmailChangeRequest request)
        {
            try
            {
                await _userService.RequestEmailChangeAsync(CurrentUserId, request.NewEmail);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange([FromBody] ConfirmEmailChangeRequest request)
        {
            try
            {
                await _userService.EmailChangeConfirmAsync(request.Token);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
