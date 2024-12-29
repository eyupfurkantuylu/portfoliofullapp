using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Contact;

public class CreateContactDto
{
    [Required(ErrorMessage = "Email adresi zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string Tel { get; set; }

    [Required(ErrorMessage = "Profil ID zorunludur")]
    public string ProfileId { get; set; }
}