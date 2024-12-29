using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Identity;

public class CreateRoleDto
{
    [Required(ErrorMessage = "Rol adı zorunludur")]
    [StringLength(50, ErrorMessage = "Rol adı en fazla 50 karakter olabilir")]
    public string Name { get; set; }
}