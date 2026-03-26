using Marketify.Entites;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Marketify.Authentication
{
    public class JwtProvider : IJwtProvider
    {
        public (string token, int expiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims =
                [new (JwtRegisteredClaimNames.Sub ,user.Id),
                new (JwtRegisteredClaimNames.Email ,user.Email!),
                new(JwtRegisteredClaimNames.GivenName ,user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName ,user.LastName),
                new(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString()),

            ];
            var symmetricSecurityKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes("OPZWDNhu7aIsv0ulpa9qJ5cy9bHr1cSz"));
            var signinCredentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);
            //معاناها بقول خد الكي دا والالجوزراميه دي عشان نعمل التوكن
            // toDo
            var expiresIn = 30;
            var expiresInDate  = DateTime.UtcNow.AddMinutes(expiresIn);
            var token = new JwtSecurityToken // شكل التوكن
                (
                issuer: "AmrElDeeb_App"
               , audience: "Marktify Users"
               , claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresIn),
               signingCredentials: signinCredentials


                );
            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn);

        }
    }
}
