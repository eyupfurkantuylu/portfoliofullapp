using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Hackathon;

public class UpdateHackathonDto
{
    [Required(ErrorMessage = "ID zorunludur")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Hackathon başlığı zorunludur")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Tarih bilgisi zorunludur")]
    public string Dates { get; set; }

    [Required(ErrorMessage = "Konum bilgisi zorunludur")]
    public string Location { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur")]
    public string Description { get; set; }

    [Url(ErrorMessage = "Geçerli bir resim URL'i giriniz")]
    public string Image { get; set; }

    [Url(ErrorMessage = "Geçerli bir MLH URL'i giriniz")]
    public string Mlh { get; set; }

    public string Win { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}