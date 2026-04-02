using Marketify.Contracts.Authenthication;
using Marketify.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Marketify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService auth) : ControllerBase
    {
        private readonly IAuthService _authService = auth;

        [HttpPost("")]
        public async Task<IActionResult>LoginAsync(LoginRequest request,CancellationToken cancellationToken = default)
        {
            var authResult = await _authService.GetTokenAsync(request.Email, request.Password);
            return authResult is null ? BadRequest("Invalid Email/password") : Ok(authResult);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestUser model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(model, cancellationToken);

            if (!string.IsNullOrEmpty(result))
            {
                return BadRequest(new { message = result });
            }

            return Ok(new { message = "Registration successful. Please check your email to confirm your account." });
        }
        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var error = await _authService.ConfirmEmailAsync(model);

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new { message = error });
            }

            return Ok(new { message = "Your email has been confirmed successfully! You can now log in." });
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPassword model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var message  = await _authService.ForgotPasswordAsync(model.Email);
            return Ok(new { message = message });
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPassword model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(model);

                return Ok(new { message = result });

        }
    }
}
