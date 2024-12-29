using Microsoft.AspNetCore.Identity;

namespace PortfolioFullApp.Core.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
    }
}