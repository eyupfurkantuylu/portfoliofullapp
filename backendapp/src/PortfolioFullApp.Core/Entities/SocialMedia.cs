namespace PortfolioFullApp.Core.Entities
{
    public class SocialMedia : BaseEntity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public bool Navbar { get; set; }
        public int Order { get; set; }
    }
}