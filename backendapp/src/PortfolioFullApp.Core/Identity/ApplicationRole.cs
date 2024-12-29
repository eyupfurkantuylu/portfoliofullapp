using Microsoft.AspNetCore.Identity;

namespace PortfolioFullApp.Core.Identity
{
    public class ApplicationRole : IdentityRole
    {
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}