namespace PortfolioFullApp.Core.Entities
{
    public class Education : BaseEntity
    {
        public string School { get; set; }
        public string Href { get; set; }
        public string Degree { get; set; }
        public string LogoUrl { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public int Order { get; set; }
    }
}