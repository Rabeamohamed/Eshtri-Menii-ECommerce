using ECom.Core.Entities;


namespace ECom.Core.Services
{
    public interface IGenerateToken
    {
        string GetAndGenerateToken(AppUser user);
    }
}
