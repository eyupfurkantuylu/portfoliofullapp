using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Identity;

public class CreateUserDto
{
    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    [StringLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olabilir")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email adresi zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre zorunludur")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
        ErrorMessage = "Şifre en az bir küçük harf, bir büyük harf, bir rakam ve bir özel karakter içermelidir")]
    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; }

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    public string PhoneNumber { get; set; }
}