using Bookify.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bookify.Services
{
    public class AuthorizationService
    {
        private readonly byte[] privateKeyBytes;
        private readonly double tokenExpirationMinutes;
        public AuthorizationService(IConfiguration configuration)
        {
            var privateKey = configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");

            privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            tokenExpirationMinutes = double.Parse(configuration["Jwt:ExpireMinutes"] ?? "60");
        }

        public string CreateToken(User user)
        {
            var handler = new JwtSecurityTokenHandler();

            var credentials = new SigningCredentials(new SymmetricSecurityKey(privateKeyBytes),
                SecurityAlgorithms.HmacSha256);

            var tokenDescription = new SecurityTokenDescriptor
            {
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
                Subject = GenerateClaims(user)
            };

            var token = handler.CreateToken(tokenDescription);
            return handler.WriteToken(token);
        }

        private ClaimsIdentity GenerateClaims(User user)
        {
            var Claims = new ClaimsIdentity();

            Claims.AddClaim(new Claim(ClaimTypes.Name, user.Name));
            Claims.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            Claims.AddClaim(new Claim("username", user.Username));
            Claims.AddClaim(new Claim("id", user.Id.ToString()));

            return Claims;
        }

        public bool IsTokenValid(string token)
        {
            token = token.Replace("Bearer ", "").Trim();
            var handler = new JwtSecurityTokenHandler();
            var param = new TokenValidationParameters();
            param.ValidateIssuer = false;
            param.ValidateAudience = false;
            param.ValidateLifetime = true;
            param.IssuerSigningKey = new SymmetricSecurityKey(privateKeyBytes);

            SecurityToken securityToken;
            try
            {
                var claims = handler.ValidateToken(token, param, out securityToken);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
