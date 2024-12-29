namespace PortfolioFullApp.Core.Entities
{
    public class ProjectLink : BaseEntity
    {
        public string Type { get; set; }
        public string Href { get; set; }
        public string Icon { get; set; }
        public string ProjectId { get; set; }
        public Project Project { get; set; }
        public int Order { get; set; }
    }
}