namespace PortfolioFullApp.Core.Entities
{
    public class HackathonLink : BaseEntity
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Href { get; set; }
        public string HackathonId { get; set; }
        public Hackathon Hackathon { get; set; }
        public int Order { get; set; }
    }
}