using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace Prog.Services
{
    public class AuthJwt : IAuthJwt
    {
        public string LogIn(string login, string password)
        {
            // Генерация JWT-токена
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                expires: DateTime.UtcNow.AddMinutes(10), 
                signingCredentials: new SigningCredentials(
                    AuthOptions.GetKey(), 
                    SecurityAlgorithms.HmacSha256 
                )
            );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}