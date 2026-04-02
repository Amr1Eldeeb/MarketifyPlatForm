using Marketify.Contracts.Authenthication;

namespace Marketify.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> GetTokenAsync(string Email, string password, CancellationToken cancellationToken =default);
        Task<string>RegisterAsync(RegisterRequestUser model , CancellationToken cancellationToken = default);

        Task<string> ConfirmEmailAsync(ConfirmEmailRequest model);
        Task<string> ForgotPasswordAsync(ForgotPassword email);
        Task<string> ResetPasswordAsync(ResetPassword request);
    }
}
