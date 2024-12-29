using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Contact;

public class UpdateContactDto
{
    [Required(ErrorMessage = "ID zorunludur")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Email adresi zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string Tel { get; set; }
}