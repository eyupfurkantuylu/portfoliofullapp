using System.Collections.Generic;
using PortfolioFullApp.Core.DTOs.HackathonLink;

namespace PortfolioFullApp.Core.DTOs.Hackathon;

public class HackathonDto : BaseDto
{
    public string Title { get; set; }
    public string Dates { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }
    public string Mlh { get; set; }
    public string Win { get; set; }
    public List<HackathonLinkDto> Links { get; set; }
    public int Order { get; set; }
}