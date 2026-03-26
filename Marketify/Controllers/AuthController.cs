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
    }
}
