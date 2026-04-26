using ECom.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ECom.Application.Interfaces.Services;

namespace ECom.Infrastructure.Service
{
    public class GenerateToken : IGenerateToken
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public GenerateToken(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<string> GetAndGenerateToken(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            // Add each role as a claim
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var Security = _configuration["Token:Secret"];
            var key = Encoding.UTF8.GetBytes(Security!);

            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor() 
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _configuration["Token:Issuer"],
                SigningCredentials = credentials,
                NotBefore = DateTime.UtcNow
            };
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
