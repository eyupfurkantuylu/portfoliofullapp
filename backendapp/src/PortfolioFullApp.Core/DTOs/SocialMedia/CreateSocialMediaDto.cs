using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.SocialMedia;

public class CreateSocialMediaDto
{
    [Required(ErrorMessage = "Sosyal medya adı zorunludur")]
    [StringLength(50, ErrorMessage = "Sosyal medya adı en fazla 50 karakter olabilir")]
    public string Name { get; set; }

    [Required(ErrorMessage = "URL zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Url { get; set; }

    [Required(ErrorMessage = "İkon zorunludur")]
    public string Icon { get; set; }

    public bool Navbar { get; set; }

    [Required(ErrorMessage = "Contact ID zorunludur")]
    public string ContactId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}