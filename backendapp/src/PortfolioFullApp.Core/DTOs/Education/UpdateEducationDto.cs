using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Education;

public class UpdateEducationDto
{
    [Required(ErrorMessage = "ID zorunludur")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Okul adı zorunludur")]
    public string School { get; set; }

    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Href { get; set; }

    [Required(ErrorMessage = "Derece/Bölüm bilgisi zorunludur")]
    public string Degree { get; set; }

    [Url(ErrorMessage = "Geçerli bir logo URL'i giriniz")]
    public string LogoUrl { get; set; }

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
    public string Start { get; set; }

    public string End { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}