namespace PortfolioFullApp.Core.DTOs.HackathonLink;

public class HackathonLinkDto : BaseDto
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public string Href { get; set; }
    public string HackathonId { get; set; }
    public int Order { get; set; }
}