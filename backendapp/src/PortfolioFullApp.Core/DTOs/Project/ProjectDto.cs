using System.Collections.Generic;
using PortfolioFullApp.Core.DTOs.ProjectLink;

namespace PortfolioFullApp.Core.DTOs.Project;

public class ProjectDto : BaseDto
{
    public string Title { get; set; }
    public string Href { get; set; }
    public string Dates { get; set; }
    public bool Active { get; set; }
    public string Description { get; set; }
    public List<string> Technologies { get; set; }
    public List<ProjectLinkDto> Links { get; set; }
    public string Image { get; set; }
    public string Video { get; set; }
    public int Order { get; set; }
}