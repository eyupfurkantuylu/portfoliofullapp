namespace PortfolioFullApp.Core.DTOs;

public abstract class BaseDto
{
    public string Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}