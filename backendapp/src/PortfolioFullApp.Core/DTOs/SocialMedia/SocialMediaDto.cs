namespace PortfolioFullApp.Core.DTOs.SocialMedia;

public class SocialMediaDto : BaseDto
{
    public string Name { get; set; }
    public string Url { get; set; }
    public string Icon { get; set; }
    public bool Navbar { get; set; }
    public string ContactId { get; set; }
    public int Order { get; set; }
}