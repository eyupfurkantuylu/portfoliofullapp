using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Identity;

public class UpdateUserDto
{
    [Required(ErrorMessage = "ID zorunludur")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email adresi zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string PhoneNumber { get; set; }

    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
}