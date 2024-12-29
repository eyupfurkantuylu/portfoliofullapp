using System.Collections.Generic;

namespace PortfolioFullApp.Core.Entities
{
    public class Profile : BaseEntity
    {
        public string Name { get; set; }
        public string Initials { get; set; }
        public string Url { get; set; }
        public string Location { get; set; }
        public string LocationLink { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string AvatarUrl { get; set; }
    }
}