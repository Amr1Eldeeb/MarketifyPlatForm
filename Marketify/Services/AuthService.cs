using Marketify.Contracts.Authenthication;
using Marketify.Entites;
using Microsoft.AspNetCore.Identity;

namespace Marketify.Services
{
    public class AuthService(UserManager<ApplicationUser>usermanger) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManger = usermanger;

        public async Task<AuthResponse?> GetTokenAsync(string Email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManger.FindByEmailAsync(Email);

            if (user == null) return null;

           var isValid =  await _userManger.CheckPasswordAsync(user, password);
            if (!isValid) return null;
            //generate Jwt Token
            
            return new AuthResponse(user.Id,user.Email,user.FirstName,user.LastName,
                "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.KMUFsIDTnFmyG3nMiGM6H9FNFUROf3wh7SmqJp-QV30",3600);
        }
    }
}
