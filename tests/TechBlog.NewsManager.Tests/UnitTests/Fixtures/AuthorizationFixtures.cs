using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechBlog.NewsManager.API.Domain.Authentication;

namespace TechBlog.NewsManager.Tests.UnitTests.Fixtures
{
    public class AuthorizationFixtures
    {
        public AccessTokenModel GenerateViewModel(bool valid, Guid? userId = null)
        {
            var id = userId is null ? Guid.NewGuid().ToString() : userId.Value.ToString();
            return valid ?
                new AccessTokenModel("Bearer", GenerateFakeJwtToken(), DateTime.Now.AddDays(1), id) :
                new AccessTokenModel();
        }

        public static string GenerateFakeJwtToken()
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())), SecurityAlgorithms.HmacSha256),
                Audience = "audience",
                Issuer = "issuer",
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
