using Marketify.Contracts.Authenthication;

namespace Marketify.Services
{
    public interface IAuthService
    {
        Task<AuthResponse?> GetTokenAsync(string Email, string password, CancellationToken cancellationToken =default);

    }
}
