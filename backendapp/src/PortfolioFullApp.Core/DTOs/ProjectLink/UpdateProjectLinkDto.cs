using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.ProjectLink;

public class UpdateProjectLinkDto
{
    [Required(ErrorMessage = "ID zorunludur")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Link tipi zorunludur")]
    public string Type { get; set; }

    [Required(ErrorMessage = "URL zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Href { get; set; }

    [Required(ErrorMessage = "İkon zorunludur")]
    public string Icon { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}