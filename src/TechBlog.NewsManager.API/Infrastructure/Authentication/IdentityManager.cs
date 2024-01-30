using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechBlog.NewsManager.API.Domain.Authentication;
using TechBlog.NewsManager.API.Domain.Entities;
using TechBlog.NewsManager.API.Domain.Logger;

namespace TechBlog.NewsManager.API.Infrastructure.Identity
{
    public class IdentityManager : IIdentityManager
    {
        private readonly UserManager<BlogUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILoggerManager _logger;

        public IdentityManager(UserManager<BlogUser> userManager, IConfiguration configuration, ILoggerManager logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> CreateUserAsync(BlogUser user, string password, CancellationToken cancellationToken)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                _logger.Log("Error creating user", LoggerManagerSeverity.INFORMATION);
                return false;
            }

            await _userManager.AddClaimsAsync(user, new[]
            {
                new Claim("BlogUserType", Enum.GetName(user.BlogUserType))
            });

            return result.Succeeded;
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
        {
            return (await GetByEmailAsync(email, cancellationToken)).Exists;
        }

        public async Task<BlogUser> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = await _userManager.FindByNameAsync(email);

            if (user is null)
                user = await _userManager.FindByEmailAsync(email);

            return user is not null ? user.WithInternalIdMapped() : new BlogUser(false);
        }

        public async Task<AccessTokenModel> AuthenticateAsync(BlogUser user, string password, CancellationToken cancellationToken, params (string name, string value)[] customClaims)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!await ValidateLoginAsync(user, password))
                return new AccessTokenModel();

            var claims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var (Name, Value) in customClaims)
                claims.Add(new Claim(Name, Value));

            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));

            foreach (var userRole in userRoles)
                claims.Add(new Claim("role", userRole));

            _logger.Log("Valid login", LoggerManagerSeverity.DEBUG, ("username", user.Email));

            return GenerateToken(user, new ClaimsIdentity(claims));
        }

        private async Task<bool> ValidateLoginAsync(BlogUser user, string password)
        {
            if (user is null)
            {
                _logger.Log("User don't exists", LoggerManagerSeverity.INFORMATION);

                return false;
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                _logger.Log("Invalid password", LoggerManagerSeverity.INFORMATION, ("username", user.Email));

                return false;
            }

            return true;
        }

        private static long ToUnixEpochDate(DateTime date)
           => (long)Math.Round((date.ToUniversalTime() - DateTimeOffset.UnixEpoch).TotalSeconds);

        private AccessTokenModel GenerateToken(BlogUser user, ClaimsIdentity claims)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials =
                    new SigningCredentials(
                        new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"],
                Expires = DateTime.UtcNow.AddSeconds(300)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AccessTokenModel("Bearer", tokenHandler.WriteToken(token), tokenDescriptor.Expires.Value, user.InternalId.ToString());
        }
    }
}
