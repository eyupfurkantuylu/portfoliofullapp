namespace PortfolioFullApp.Core.DTOs.NavbarItem;

public class NavbarItemDto : BaseDto
{
    public string Href { get; set; }
    public string Icon { get; set; }
    public string Label { get; set; }
    public string ProfileId { get; set; }
    public int Order { get; set; }
}