using System.Collections.Generic;

namespace PortfolioFullApp.Core.Entities
{
    public class Hackathon : BaseEntity
    {
        public string Title { get; set; }
        public string Dates { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Mlh { get; set; }
        public string Win { get; set; }
        public List<HackathonLink> Links { get; set; }
        public int Order { get; set; }
    }
}