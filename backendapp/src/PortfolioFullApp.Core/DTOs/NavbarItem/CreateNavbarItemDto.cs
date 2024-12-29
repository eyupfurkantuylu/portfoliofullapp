using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.NavbarItem;

public class CreateNavbarItemDto
{
    [Required(ErrorMessage = "URL zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Href { get; set; }

    [Required(ErrorMessage = "İkon zorunludur")]
    public string Icon { get; set; }

    [Required(ErrorMessage = "Etiket zorunludur")]
    public string Label { get; set; }

    [Required(ErrorMessage = "Profil ID zorunludur")]
    public string ProfileId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}