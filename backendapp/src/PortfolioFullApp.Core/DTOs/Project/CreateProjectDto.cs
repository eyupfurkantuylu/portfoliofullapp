using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PortfolioFullApp.Core.DTOs.Project;

public class CreateProjectDto
{
    [Required(ErrorMessage = "Proje başlığı zorunludur")]
    public string Title { get; set; }

    [Required(ErrorMessage = "URL zorunludur")]
    [Url(ErrorMessage = "Geçerli bir URL giriniz")]
    public string Href { get; set; }

    [Required(ErrorMessage = "Tarih bilgisi zorunludur")]
    public string Dates { get; set; }

    public bool Active { get; set; }

    [Required(ErrorMessage = "Açıklama zorunludur")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Teknolojiler zorunludur")]
    public string Technologies { get; set; }

    [Url(ErrorMessage = "Geçerli bir resim URL'i giriniz")]
    public string Image { get; set; }

    [Url(ErrorMessage = "Geçerli bir video URL'i giriniz")]
    public string Video { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sıralama değeri 1'den büyük olmalıdır")]
    public int Order { get; set; }
}