using ECom.Core.Entities;


namespace ECom.Application.Services
{
    public interface IGenerateToken
    {
        Task<string> GetAndGenerateToken(AppUser user);
    }
}
