using System.Collections.Generic;

namespace PortfolioFullApp.Core.DTOs.Work;

public class WorkDto : BaseDto
{
    public string Company { get; set; }
    public string Href { get; set; }
    public List<string> Badges { get; set; }
    public string Location { get; set; }
    public string Title { get; set; }
    public string LogoUrl { get; set; }
    public string Start { get; set; }
    public string End { get; set; }
    public string Description { get; set; }
    public int Order { get; set; }
}