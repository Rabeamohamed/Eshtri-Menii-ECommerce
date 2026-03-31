using Microsoft.AspNetCore.Identity;

namespace ECom.Core.Entities
{
    public class AppUser :IdentityUser
    {
        public string DisplayName { get; set; }
        public Address Address { get; set; }
        public bool IsBlocked { get; set; } = false;      
        public string BlockReason { get; set; }            
        public DateTime? BlockedAt { get; set; }
    }
}
