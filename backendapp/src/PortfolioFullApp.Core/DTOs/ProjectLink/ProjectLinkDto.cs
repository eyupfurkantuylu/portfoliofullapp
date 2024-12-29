namespace PortfolioFullApp.Core.DTOs.ProjectLink;

public class ProjectLinkDto : BaseDto
{
    public string Type { get; set; }
    public string Href { get; set; }
    public string Icon { get; set; }
    public string ProjectId { get; set; }
    public int Order { get; set; }
}