using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.HackathonLink;

public class UpdateHackathonLinkDto
{
    [Required(ErrorMessage = "ID zorunludur")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur")]
    public string Title { get; set; }

    [Required(ErrorMessage = "İkon zorunludur")]
    public string Icon { get; set; }

    [Required(ErrorMessage = "URL zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Href { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}