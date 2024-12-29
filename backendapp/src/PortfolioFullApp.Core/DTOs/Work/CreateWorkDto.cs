using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Work;

public class CreateWorkDto
{
    [Required(ErrorMessage = "Şirket adı zorunludur")]
    [StringLength(100, ErrorMessage = "Şirket adı en fazla 100 karakter olabilir")]
    public string Company { get; set; }

    [Required(ErrorMessage = "URL zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Href { get; set; }

    public List<string> Badges { get; set; }

    [Required(ErrorMessage = "Konum zorunludur")]
    public string Location { get; set; }

    [Required(ErrorMessage = "Pozisyon başlığı zorunludur")]
    [StringLength(100, ErrorMessage = "Pozisyon başlığı en fazla 100 karakter olabilir")]
    public string Title { get; set; }

    [Url(ErrorMessage = "Geçerli bir logo URL'i giriniz")]
    public string LogoUrl { get; set; }

    [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
    public string Start { get; set; }

    public string End { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur")]
    public string Description { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}