using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using PortfolioFullApp.Core.DTOs.SocialMedia;

namespace PortfolioFullApp.Core.DTOs.Contact;

public class CreateContactWithSocialMediaDto
{
    [Required(ErrorMessage = "Email adresi zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string Tel { get; set; }

    public List<CreateSocialMediaForContactDto> Social { get; set; } = new();
}

public class CreateSocialMediaForContactDto
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

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}