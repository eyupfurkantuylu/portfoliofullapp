using System.Collections.Generic;
using PortfolioFullApp.Core.DTOs.SocialMedia;

namespace PortfolioFullApp.Core.DTOs.Contact;

public class ContactDto : BaseDto
{
    public string Email { get; set; }
    public string Tel { get; set; }
    public List<SocialMediaDto> Social { get; set; }
}