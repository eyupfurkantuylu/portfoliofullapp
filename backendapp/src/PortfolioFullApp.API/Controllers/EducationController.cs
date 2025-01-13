using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PortfolioFullApp.Core.Common;
using PortfolioFullApp.Core.DTOs.Education;
using PortfolioFullApp.Core.Entities;
using PortfolioFullApp.Core.Interfaces;

namespace backendapp.src.PortfolioFullApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EducationController : ControllerBase
    {
        private readonly IEducationRepository _educationRepository;

        public EducationController(IEducationRepository educationRepository)
        {
            _educationRepository = educationRepository;
        }

        /// <summary>
        /// Tüm eğitim bilgilerini getirir
        /// </summary>
        /// <returns>Eğitim bilgileri listesi</returns>
        /// <response code="200">Eğitim bilgileri başarıyla getirildi</response>
        /// <response code="400">İstek sırasında bir hata oluştu</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<EducationDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<IEnumerable<EducationDto>>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<IEnumerable<EducationDto>>>> GetAll()
        {
            try
            {
                var educations = await _educationRepository.GetAllAsync();
                return Ok(ApiResult<IEnumerable<EducationDto>>.SuccessResult(educations, "Eğitim bilgileri başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<IEnumerable<EducationDto>>.ErrorResult("Eğitim bilgileri getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip eğitim bilgisini getirir
        /// </summary>
        /// <param name="id">Eğitim bilgisi ID'si</param>
        /// <returns>Eğitim bilgisi</returns>
        /// <response code="200">Eğitim bilgisi başarıyla getirildi</response>
        /// <response code="404">Eğitim bilgisi bulunamadı</response>
        /// <response code="400">İstek sırasında bir hata oluştu</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<EducationDto>>> GetById(string id)
        {
            try
            {
                var education = await _educationRepository.GetByIdAsync(id);
                if (education == null)
                {
                    return NotFound(ApiResult<EducationDto>.ErrorResult("Eğitim bilgisi bulunamadı"));
                }
                return Ok(ApiResult<EducationDto>.SuccessResult(education, "Eğitim bilgisi başarıyla getirildi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<EducationDto>.ErrorResult("Eğitim bilgisi getirilirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Yeni bir eğitim bilgisi oluşturur
        /// </summary>
        /// <param name="createEducationDto">Oluşturulacak eğitim bilgisi</param>
        /// <returns>Oluşturulan eğitim bilgisi</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<EducationDto>>> Create([FromBody] CreateEducationDto createEducationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<EducationDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var education = new Education
                {
                    Id = Guid.NewGuid().ToString(),
                    School = createEducationDto.School,
                    Href = createEducationDto.Href,
                    Degree = createEducationDto.Degree,
                    LogoUrl = createEducationDto.LogoUrl,
                    Start = createEducationDto.Start,
                    End = createEducationDto.End,
                    Order = createEducationDto.Order
                };

                var createdEducation = await _educationRepository.CreateAsync(education);
                return CreatedAtAction(nameof(GetById),
                    new { id = createdEducation.Id },
                    ApiResult<EducationDto>.SuccessResult(createdEducation, "Eğitim bilgisi başarıyla oluşturuldu"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<EducationDto>.ErrorResult("Eğitim bilgisi oluşturulurken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Mevcut bir eğitim bilgisini günceller
        /// </summary>
        /// <param name="updateEducationDto">Güncellenecek eğitim bilgisi</param>
        /// <returns>Güncellenen eğitim bilgisi</returns>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<EducationDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<EducationDto>>> Update([FromBody] UpdateEducationDto updateEducationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResult<EducationDto>.ErrorResult("Geçersiz model durumu",
                        ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
                }

                var education = new Education
                {
                    Id = updateEducationDto.Id,
                    School = updateEducationDto.School,
                    Href = updateEducationDto.Href,
                    Degree = updateEducationDto.Degree,
                    LogoUrl = updateEducationDto.LogoUrl,
                    Start = updateEducationDto.Start,
                    End = updateEducationDto.End,
                    Order = updateEducationDto.Order
                };

                var updatedEducation = await _educationRepository.UpdateAsync(education);
                if (updatedEducation == null)
                {
                    return NotFound(ApiResult<EducationDto>.ErrorResult("Güncellenecek eğitim bilgisi bulunamadı"));
                }

                return Ok(ApiResult<EducationDto>.SuccessResult(updatedEducation, "Eğitim bilgisi başarıyla güncellendi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<EducationDto>.ErrorResult("Eğitim bilgisi güncellenirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Belirtilen ID'ye sahip eğitim bilgisini siler
        /// </summary>
        /// <param name="id">Silinecek eğitim bilgisi ID'si</param>
        /// <returns>Silme işlemi sonucu</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult<bool>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResult<bool>>> Delete(string id)
        {
            try
            {
                var result = await _educationRepository.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(ApiResult<bool>.ErrorResult("Silinecek eğitim bilgisi bulunamadı"));
                }

                return Ok(ApiResult<bool>.SuccessResult(true, "Eğitim bilgisi başarıyla silindi"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResult<bool>.ErrorResult("Eğitim bilgisi silinirken bir hata oluştu",
                    new List<string> { ex.Message }));
            }
        }
    }
}