using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Skill;

public class UpdateSkillDto
{
    [Required(ErrorMessage = "ID zorunludur")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Yetenek adı zorunludur")]
    [StringLength(50, ErrorMessage = "Yetenek adı en fazla 50 karakter olabilir")]
    public string Name { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}