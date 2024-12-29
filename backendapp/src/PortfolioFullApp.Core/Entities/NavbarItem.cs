namespace PortfolioFullApp.Core.Entities
{
    public class NavbarItem : BaseEntity
    {
        public string Href { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public string ProfileId { get; set; }
        public Profile Profile { get; set; }
        public int Order { get; set; }
    }
}