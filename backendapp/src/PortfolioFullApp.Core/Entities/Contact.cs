using System.Collections.Generic;

namespace PortfolioFullApp.Core.Entities
{
    public class Contact : BaseEntity
    {
        public string Email { get; set; }
        public string Tel { get; set; }
        public List<SocialMedia> Social { get; set; }
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
    }
}