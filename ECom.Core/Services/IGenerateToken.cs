using ECom.Core.Entities;


namespace ECom.Core.Services
{
    public interface IGenerateToken
    {
        Task<string> GetAndGenerateToken(AppUser user);
    }
}
