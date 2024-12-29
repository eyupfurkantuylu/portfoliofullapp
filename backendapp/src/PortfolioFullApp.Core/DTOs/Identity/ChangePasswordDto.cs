using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Identity;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Mevcut şifre zorunludur")]
    public string CurrentPassword { get; set; }

    [Required(ErrorMessage = "Yeni şifre zorunludur")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6, en fazla 100 karakter olmalıdır")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{6,}$",
        ErrorMessage = "Şifre en az bir küçük harf, bir büyük harf, bir rakam ve bir özel karakter içermelidir")]
    public string NewPassword { get; set; }

    [Compare("NewPassword", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmNewPassword { get; set; }
}