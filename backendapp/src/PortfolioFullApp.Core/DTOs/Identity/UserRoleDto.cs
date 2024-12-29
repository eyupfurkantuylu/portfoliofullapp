using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Identity;

public class UserRoleDto
{
    [Required(ErrorMessage = "Kullanıcı ID zorunludur")]
    public string UserId { get; set; }

    [Required(ErrorMessage = "Rol ID zorunludur")]
    public string RoleId { get; set; }
}