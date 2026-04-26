using ECom.Core.Entities;


namespace ECom.Application.Interfaces.Services
{
    public interface IGenerateToken
    {
        Task<string> GetAndGenerateToken(AppUser user);
        string GenerateRefreshToken();
    }
}
