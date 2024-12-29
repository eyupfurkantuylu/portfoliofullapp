using System.Collections.Generic;
using PortfolioFullApp.Core.DTOs.Contact;
using PortfolioFullApp.Core.DTOs.Skill;
using PortfolioFullApp.Core.DTOs.NavbarItem;

namespace PortfolioFullApp.Core.DTOs.Profile;

public class ProfileDto : BaseDto
{
    public string Name { get; set; }
    public string Initials { get; set; }
    public string Url { get; set; }
    public string Location { get; set; }
    public string LocationLink { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string AvatarUrl { get; set; }
}