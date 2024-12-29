namespace PortfolioFullApp.Core.Entities
{
    public class NavbarItem : BaseEntity
    {
        public string Href { get; set; }
        public string Icon { get; set; }
        public string Label { get; set; }
        public int Order { get; set; }
    }
}