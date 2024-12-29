using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Profile;

public class CreateProfileDto
{
    [Required(ErrorMessage = "İsim zorunludur")]
    [StringLength(100, ErrorMessage = "İsim en fazla 100 karakter olabilir")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Baş harfler zorunludur")]
    [StringLength(10, ErrorMessage = "Baş harfler en fazla 10 karakter olabilir")]
    public string Initials { get; set; }

    [Required(ErrorMessage = "URL zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Url { get; set; }

    [Required(ErrorMessage = "Konum zorunludur")]
    public string Location { get; set; }

    [Url(ErrorMessage = "Geçerli bir konum URL'i giriniz")]
    public string LocationLink { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Özet zorunludur")]
    public string Summary { get; set; }

    [Url(ErrorMessage = "Geçerli bir avatar URL'i giriniz")]
    public string AvatarUrl { get; set; }
}